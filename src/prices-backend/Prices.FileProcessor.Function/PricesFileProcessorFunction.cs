using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using Prices.Core.Application.Exceptions;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Models;
using Prices.Persistence.EntityFramework;
using Prices.Persistence.EntityFramework.Extensions;
using Serilog;

namespace Prices.FileProcessor.Function
{
    public class PricesFileProcessorFunction
    {
        private readonly IDbContextFactory<PricesContext> _contextFactory;
        private readonly IPricesFileProcessorFactory _pricesFileProcessorFactory;
        private readonly IClock _clock;
        private readonly ILogger _logger;

        public PricesFileProcessorFunction(
            IDbContextFactory<PricesContext> contextFactory,
            IPricesFileProcessorFactory pricesFileProcessorFactory,
            IClock clock,
            ILogger logger)
        {
            _contextFactory = contextFactory;
            _pricesFileProcessorFactory = pricesFileProcessorFactory;
            _clock = clock;
            _logger = logger;
        }

        [FunctionName("ProcessPricesFile")]
        //[FixedDelayRetry(5, "00:01:00")]
        public async Task ProcessPricesFile([BlobTrigger("prices/{name}", Connection = "Prices", Source = BlobTriggerSource.EventGrid)]Stream stream, 
            string name, IDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            var fileSize = stream.Length;
            _logger.Information("C# Blob trigger function Processed blob\n Name:{name} \n Size: {length} Bytes", name, fileSize);

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
                await context.BulkSavePricesAsync(blobMetadata, name, fileSize, now, result.Prices, cancellationToken);
                _logger.Information("Inserted {pricesCount} prices for {blobMetadata} {minDate}",
                    result.Prices.Count(), blobMetadata.ToString(), $"{result.Prices.Min(p => p.IntervalStartTimeUtc):%yyyy-MM-dd}");
            }
            catch (Exception ex)
            {
                _logger.Error("Unhandled exception processing {name}: {exMessage}", name, ex.Message);
                throw;
            }
        }
    }
}
