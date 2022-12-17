using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Helpers;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Downloader.Services.Services;

public class ErcotHistoricalPricesFileDownloader : ErcotPricesFileDownloader, IHistoricalPricesFileDownloader
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;
    private readonly ILogger<ErcotHistoricalPricesFileDownloader> _logger;

    public ErcotHistoricalPricesFileDownloader(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
        IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<ErcotHistoricalPricesFileDownloader> logger, IOptions<Settings> settings)
        : base(azureBlobStorageClientFactory, contextFactory, pricesFileProcessorFactory, logger, settings)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<DownloadHistoricalPricesResult> DownloadHistoricalPricesAsync(int year, int? _, IEnumerable<string>? nodes = null,
        int retryAttempts = 2, int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default)
    {
        var nodesList = nodes?.ToList();
        var historicalPricesFiles = await GetExistingPricesFiles(RegionalTransmissionOperator, PriceTypes.Historical, cancellationToken);

        var priceIndexes = await GetDefaultPriceIndexesToDownloadPricesFor(cancellationToken);
        foreach (var priceIndex in priceIndexes)
        {
            var date = LocalDate.FromDateTime(new DateTime(year, 1, 1));
            var utcDate = date.AtStartOfDayInZone(TimeZoneHelper.GetDateTimeZone(Rtos.ERCOT)).ToInstant();

            if (historicalPricesFiles.Any(f => f.PriceIndexId == priceIndex.Id && f.StartDateUtc == utcDate) &&
                year != DateTime.Today.Year && year != DateTime.Today.Year - 1)
            {
                continue;
            }

            var filesToDownloadResult = await GetFilesToDownload(PriceTypes.Historical, priceIndex, FileFormats.Excel, date, null, nodesList, cancellationToken);
            if (!filesToDownloadResult.Success)
                return new DownloadHistoricalPricesResult(false, filesToDownloadResult.Errors);

            var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
            if (fileToDownload is null)
            {
                var errors = new[] { $"Unable to get the single file historical prices file for {RegionalTransmissionOperator} {priceIndex.PriceMarketId} {date:yyyy}" };
                return new DownloadHistoricalPricesResult(false, errors);
            }

            _logger.LogInformation("Downloading {rto} {priceMarketId} {date} prices", RegionalTransmissionOperator, priceIndex.PriceMarketId, $"{date:yyyy}");

            var result = await DownloadPricesFileAsync(fileToDownload, retryAttempts, delayInSecondsBetweenRetryAttempts, cancellationToken);
            if (!result.Success)
                return new DownloadHistoricalPricesResult(false, result.Errors);
        }

        return new DownloadHistoricalPricesResult(true, Array.Empty<string>());
    }

    private async Task<List<PriceIndex>> GetDefaultPriceIndexesToDownloadPricesFor(CancellationToken cancellationToken)
    {
        var priceMarkets = new List<PriceMarkets>
        {
            PriceMarkets.DAM,
            PriceMarkets.RTM
        };

        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PriceIndexes
            .Where(i => i.RegionalTransmissionOperatorId == RegionalTransmissionOperator && priceMarkets.Contains(i.PriceMarketId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}