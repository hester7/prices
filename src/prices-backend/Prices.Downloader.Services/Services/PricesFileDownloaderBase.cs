using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Enums;
using Prices.Core.Application.Interfaces;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;
using System.IO.Compression;

namespace Prices.Downloader.Services.Services;

public abstract class PricesFileDownloaderBase
{
    protected readonly IAzureBlobStorageClient AzureBlobStorageClient;
    protected readonly IDbContextFactory<PricesContext> ContextFactory;
    private readonly IPricesFileProcessorFactory _pricesFileProcessorFactory;
    private readonly ILogger<PricesFileDownloaderBase> _logger;

    protected PricesFileDownloaderBase(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
        IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<PricesFileDownloaderBase> logger, IOptions<Settings> settings)
    {
        AzureBlobStorageClient = azureBlobStorageClientFactory.NewSasTokenClient(settings.Value.SasUri);
        ContextFactory = contextFactory;
        _pricesFileProcessorFactory = pricesFileProcessorFactory;
        _logger = logger;
    }

    public abstract Rtos RegionalTransmissionOperator { get; }

    protected virtual string FormatFileName(PricesFileToDownload fileToDownload, string fileName) => fileName;

    public async Task<DownloadPricesFileResult> DownloadPricesFileAsync(PricesFileToDownload fileToDownload, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default)
    {
        var result = await DownloadPricesAsync(DownloadResultsType.File, fileToDownload, retryAttempts, delayInSecondsBetweenRetryAttempts, cancellationToken);
        return new DownloadPricesFileResult(result.Success, result.Metadata, result.Errors);
    }

    public async Task<DownloadPricesIntervalsResult> DownloadPriceIntervalsAsync(PricesFileToDownload fileToDownload, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default)
    {
        var result = await DownloadPricesAsync(DownloadResultsType.Intervals, fileToDownload, retryAttempts, delayInSecondsBetweenRetryAttempts, cancellationToken);
        return new DownloadPricesIntervalsResult(result.Success, result.Prices!, result.Errors, result.Warnings);
    }

    protected async Task<DownloadPricesResult> DownloadPricesAsync(DownloadResultsType downloadResultsType, PricesFileToDownload fileToDownload,
        int retryAttempts, int delayInSecondsBetweenRetryAttempts, CancellationToken cancellationToken)
    {
        var attempts = 0;
        var errors = new List<string>();
        List<PricingNode>? pricingNodes = null;

        while (true)
        {
            try 
            {
                ++attempts;
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(10);
                using var request = new HttpRequestMessage(HttpMethod.Get, fileToDownload.RequestUri);
                using var response = await client.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var zipStream = await response.Content.ReadAsStreamAsync(cancellationToken);

                using var archive = new ZipArchive(zipStream);

                if (!archive.Entries.Any())
                {
                    errors.Add($"{fileToDownload}: The zip file in the response was empty.");
                    return new DownloadPricesResult(false, null, null, errors, Array.Empty<string>());
                }

                foreach (var entry in archive.Entries)
                {
                    await using var stream = entry.Open();
                    var metadata = new PricesFileMetadata(fileToDownload, FormatFileName(fileToDownload, entry.Name), fileToDownload.DocumentId);

                    switch (downloadResultsType)
                    {
                        case DownloadResultsType.File:
                            {
                                await AzureBlobStorageClient.UploadFileAsync(stream, metadata.RemoteFolder, metadata.FileName, metadata, cancellationToken);
                                _logger.LogInformation("Uploaded prices for {metadata} ({fileName})", metadata.ToString(), metadata.FileName);

                                return new DownloadPricesResult(true, metadata, null, Array.Empty<string>(), Array.Empty<string>());
                            }
                        case DownloadResultsType.Intervals:
                            {
                                var processor = _pricesFileProcessorFactory.GetProcessorByRtoAndFileFormat(RegionalTransmissionOperator, metadata.FileFormatId);
                                if (processor is null)
                                {
                                    errors.Add($"{fileToDownload}: There is no {metadata.FileFormatId} prices file processor for {RegionalTransmissionOperator}.");
                                    return new DownloadPricesResult(false, null, null, errors, Array.Empty<string>());
                                }

                                pricingNodes ??= await GetDefaultPricingNodesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
                                var result = processor.ProcessPrices(stream, metadata.RegionalTransmissionOperatorId,
                                    metadata.PriceMarketId, metadata.PriceIndexId, metadata.PriceTypeId, pricingNodes);
                                return new DownloadPricesResult(result.Success, null, result.Prices, result.Errors, result.Warnings);
                            }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(downloadResultsType), downloadResultsType, null);
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"{fileToDownload}: {ex.Message}");

                if (attempts > retryAttempts)
                    return new DownloadPricesResult(false, null, null, errors, Array.Empty<string>());

                await Task.Delay(delayInSecondsBetweenRetryAttempts * 1000, cancellationToken);
            }
        }
    }

    protected virtual async Task<List<PricingNode>> GetDefaultPricingNodesToDownloadPricesFor(Rtos rto, CancellationToken cancellationToken)
    {
        var pricingNodeTypes = new List<PricingNodeTypes>
        {
            PricingNodeTypes.Hub,
            PricingNodeTypes.DLAP
        };

        await using var context = await ContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PricingNodes
            .Where(pn => pn.RegionalTransmissionOperatorId == rto && pricingNodeTypes.Contains(pn.PricingNodeTypeId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    protected async Task<List<PricesFile>> GetExistingPricesFiles(Rtos rto, PriceTypes priceType, CancellationToken cancellationToken)
    {
        await using var context = await ContextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PricesFiles
            .Where(f => f.PriceIndex.RegionalTransmissionOperatorId == rto && f.PriceTypeId == priceType)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}