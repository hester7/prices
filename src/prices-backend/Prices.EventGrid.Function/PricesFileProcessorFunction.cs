// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Exceptions;
using Prices.Core.Application.Interfaces;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Models;
using Prices.Persistence.EntityFramework;
using Prices.Persistence.EntityFramework.Extensions;
using Serilog;

namespace Prices.EventGrid.Function
{
    public class PricesFileProcessorFunction
    {
        private readonly IAzureBlobStorageClient _azureBlobStorageClient;
        private readonly IDbContextFactory<PricesContext> _contextFactory;
        private readonly IPricesFileProcessorFactory _pricesFileProcessorFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public PricesFileProcessorFunction(
            IAzureBlobStorageClientFactory azureBlobStorageClientFactory,
            IDbContextFactory<PricesContext> contextFactory,
            IPricesFileProcessorFactory pricesFileProcessorFactory,
            IClock clock,
            ILogger logger,
            IOptions<Settings> settings)
        {
            _azureBlobStorageClient = azureBlobStorageClientFactory.NewSasTokenClient(settings.Value.SasUri);
            _contextFactory = contextFactory;
            _pricesFileProcessorFactory = pricesFileProcessorFactory;
            _clock = clock;
            _logger = logger;
        }

        [FunctionName("ProcessPricesFile")]
        [FixedDelayRetry(5, "00:01:00")]
        public async Task ProcessPricesFile([EventGridTrigger] EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "Prices")] Stream stream,
            CancellationToken cancellationToken)
        {
            var blobEvent = JsonConvert.DeserializeObject<BlobEventData>(eventGridEvent.Data.ToString());

            if (blobEvent == null)
            {
                const string errorMessage = "Could not parse blob event data from EventGrid event.";
                _logger.Error(errorMessage);
                return;
            }

            var splitUrl = blobEvent.Url.Split(new[] { "prices/" }, StringSplitOptions.None);
            if (splitUrl.Length <= 1)
            {
                const string errorMessage = "Could not parse blob event data from EventGrid event.";
                _logger.Error(errorMessage);
                return;
            }

            var blobName = splitUrl[1];
            var metadata = await _azureBlobStorageClient.GetBlobMetadataAsync(blobName, cancellationToken);

            var fileSize = stream.Length;
            _logger.Information("C# Blob trigger function Processed blob\n Name:{name} \n Size: {length} Bytes", blobName, fileSize);

            try
            {
                var blobMetadata = JsonConvert.DeserializeObject<PricesFileMetadata>(JsonConvert.SerializeObject(metadata))!;

                var fileProcessor = _pricesFileProcessorFactory.GetProcessorByRtoAndFileFormat(blobMetadata.RegionalTransmissionOperatorId, blobMetadata.FileFormatId);
                if (fileProcessor is null)
                {
                    _logger.Error("No prices file processor implemented for {rto}", blobMetadata.RegionalTransmissionOperatorId);
                    return;
                }

                await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
                var pricingNodes = await context.PricingNodes
                    .Where(pn => pn.RegionalTransmissionOperatorId == blobMetadata.RegionalTransmissionOperatorId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream, cancellationToken);
                memStream.Position = 0;

                var result = fileProcessor.ProcessPrices(
                    memStream,
                    blobMetadata.RegionalTransmissionOperatorId,
                    blobMetadata.PriceMarketId,
                    blobMetadata.PriceIndexId,
                    blobMetadata.PriceTypeId,
                    pricingNodes);

                if (!result.Success)
                {
                    if (result.Errors.Any())
                        throw new PricesFileProcessorFunctionException(blobMetadata, result.Errors);

                    if (result.Warnings.Any())
                        _logger.Warning("Warning processing prices for {blobMetadata} ({fileSourceUrl}): {warnings)}",
                            blobMetadata.ToString(), blobMetadata.FileSourceUrl, string.Join(Environment.NewLine, result.Warnings));

                    return;
                }

                if (!result.Prices.Any())
                {
                    _logger.Warning("File {blobMetadata} generated no price intervals", blobMetadata.ToString());
                    return;
                }

                var now = _clock.GetCurrentInstant();
                await context.BulkSavePricesAsync(blobMetadata, blobName, fileSize, now, result.Prices, cancellationToken);
                _logger.Information("Inserted {pricesCount} prices for {blobMetadata} {minDate}",
                    result.Prices.Count(), blobMetadata.ToString(), $"{result.Prices.Min(p => p.IntervalStartTimeUtc):%yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                _logger.Error("Unhandled exception processing {name}: {exMessage}", blobName, ex.Message);
                throw;
            }
        }
    }
}
