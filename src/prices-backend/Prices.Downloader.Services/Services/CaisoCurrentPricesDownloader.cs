using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Enums;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Services.Services;

public class CaisoCurrentPricesDownloader : CaisoPricesFileDownloader, ICurrentPricesDownloader
{
    private readonly IPricesFileProcessor? _processor;
    private readonly IDbContextFactory<PricesContext> _contextFactory;
    private readonly ILogger<CaisoCurrentPricesDownloader> _logger;
    private PriceIndex? _priceIndex;

    public CaisoCurrentPricesDownloader(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
        IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<CaisoCurrentPricesDownloader> logger, IOptions<Settings> settings)
        : base(azureBlobStorageClientFactory, contextFactory, pricesFileProcessorFactory, logger, settings)
    {
        _processor = pricesFileProcessorFactory.GetProcessorByRtoAndFileFormat(RegionalTransmissionOperator, FileFormats.XML);
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public sealed override Rtos RegionalTransmissionOperator => base.RegionalTransmissionOperator;

    public async Task<FileProcessorResult> DownloadCurrentPricesAsync(IEnumerable<string>? nodes = null, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default)
    {
        if (_processor is null)
        {
            var errors = new[] { $"No prices file processor implemented for {RegionalTransmissionOperator}" };
            return new FileProcessorResult(false, Array.Empty<Price>(), errors, Array.Empty<string>());
        }

        if (_priceIndex is null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            _priceIndex = await context.PriceIndexes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.RegionalTransmissionOperatorId == RegionalTransmissionOperator && i.PriceMarketId == PriceMarkets.RTM, cancellationToken);
        }

        var date = LocalDate.FromDateTime(DateTime.Today);
        var filesToDownloadResult = await GetFilesToDownload(PriceTypes.Current, _priceIndex!, FileFormats.XML, date, null, nodes, cancellationToken);
        if (!filesToDownloadResult.Success)
            return new FileProcessorResult(filesToDownloadResult.Success, Enumerable.Empty<Price>(), filesToDownloadResult.Errors, Array.Empty<string>());

        var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
        if (fileToDownload is null)
        {
            var errors = new[] { $"Unable to get the single file to download for the current prices of {RegionalTransmissionOperator} {_priceIndex!.PriceMarketId}" };
            return new FileProcessorResult(false, Enumerable.Empty<Price>(), errors, Array.Empty<string>());
        }

        _logger.LogInformation("Downloading current prices for {rto} {priceMarketId}", RegionalTransmissionOperator, _priceIndex!.PriceMarketId);

        var result = await DownloadPricesAsync(DownloadResultsType.Intervals, fileToDownload, retryAttempts, delayInSecondsBetweenRetryAttempts, cancellationToken);
        if (!result.Success)
        {
            _logger.LogError("Error downloading current prices for {rto} {priceMarketId}", RegionalTransmissionOperator, _priceIndex!.PriceMarketId);
            result.Errors.ToList().ForEach(e => _logger.LogError(e));
        }

        var prices = result.Prices ?? Enumerable.Empty<Price>();
        return new FileProcessorResult(result.Success, prices, result.Errors, Array.Empty<string>());
    }
}