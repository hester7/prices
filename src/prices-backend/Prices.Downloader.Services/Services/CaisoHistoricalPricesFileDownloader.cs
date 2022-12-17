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

public class CaisoHistoricalPricesFileDownloader : CaisoPricesFileDownloader, IHistoricalPricesFileDownloader
{
    private readonly ILogger<CaisoHistoricalPricesFileDownloader> _logger;

    public CaisoHistoricalPricesFileDownloader(IAzureBlobStorageClientFactory azureBlobStorageClientFactory, IDbContextFactory<PricesContext> contextFactory,
        IPricesFileProcessorFactory pricesFileProcessorFactory, ILogger<CaisoHistoricalPricesFileDownloader> logger, IOptions<Settings> settings)
        : base(azureBlobStorageClientFactory, contextFactory, pricesFileProcessorFactory, logger, settings)
    {
        _logger = logger;
    }

    public async Task<DownloadHistoricalPricesResult> DownloadHistoricalPricesAsync(int year, int? month, IEnumerable<string>? nodes = null,
        int retryAttempts = 2, int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default)
    {
        var nodesList = nodes?.ToList();
        var historicalPricesFiles = await GetExistingPricesFiles(RegionalTransmissionOperator, PriceTypes.Historical, cancellationToken);

        var currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var previousMonth = currentMonth.AddMonths(-1);

        var success = true;
        var errors = new List<string>();
        var priceIndexes = await GetDefaultPriceIndexesToDownloadPricesFor(RegionalTransmissionOperator, cancellationToken);
        foreach (var priceIndex in priceIndexes)
        {
            if (month is not null)
            {
                var date = new DateTime(year, month.Value, 1);
                var startDate = LocalDate.FromDateTime(date);
                var endDate = LocalDate.FromDateTime(date.AddMonths(1));

                var currentTime = DateTime.Now;
                var result = await DownloadHistoricalPricesAsync(priceIndex, startDate, endDate, nodesList, retryAttempts,
                    delayInSecondsBetweenRetryAttempts, cancellationToken);

                if (!result.Success)
                {
                    success = false;
                    errors.AddRange(result.Errors);
                }

                // CAISO Acceptable Use Policy Violation. Please retry your request after 5 seconds.
                var milliseconds = DateTime.Now.Subtract(currentTime).Milliseconds;
                if (milliseconds < 5000)
                    await Task.Delay(milliseconds + 100, cancellationToken);
            }
            else
            {
                for (var monthIterator = 1; monthIterator <= 12; monthIterator++)
                {
                    if (year == DateTime.Today.Year && monthIterator > DateTime.Today.Month)
                        continue;

                    var date = new DateTime(year, monthIterator, 1);
                    var startDate = LocalDate.FromDateTime(date);
                    var endDate = LocalDate.FromDateTime(date.AddMonths(1));
                    var utcStartDate = startDate.AtStartOfDayInZone(TimeZoneHelper.GetDateTimeZone(Rtos.CAISO)).ToInstant();

                    if (historicalPricesFiles.Any(f => f.PriceIndexId == priceIndex.Id && f.StartDateUtc == utcStartDate) &&
                        date != currentMonth && date != previousMonth)
                    {
                        continue;
                    }

                    var currentTime = DateTime.Now;
                    var result = await DownloadHistoricalPricesAsync(priceIndex, startDate, endDate, nodesList, retryAttempts,                        
                        delayInSecondsBetweenRetryAttempts, cancellationToken);

                    if (!result.Success)
                    {
                        success = false;
                        errors.AddRange(result.Errors);
                    }

                    // CAISO Acceptable Use Policy Violation. Please retry your request after 5 seconds.
                    var milliseconds = DateTime.Now.Subtract(currentTime).Milliseconds;
                    if (milliseconds < 5000)
                        await Task.Delay(milliseconds + 100, cancellationToken);
                }
            }
        }

        return new DownloadHistoricalPricesResult(success, errors);
    }

    private async Task<DownloadHistoricalPricesResult> DownloadHistoricalPricesAsync(PriceIndex priceIndex, LocalDate startDate, LocalDate endDate,
        List<string>? nodesList, int retryAttempts, int delayInSecondsBetweenRetryAttempts, CancellationToken cancellationToken)
    {
        var filesToDownloadResult = await GetFilesToDownload(PriceTypes.Historical, priceIndex, FileFormats.XML, startDate, endDate, nodesList, cancellationToken);
        if (!filesToDownloadResult.Success)
            return new DownloadHistoricalPricesResult(false, filesToDownloadResult.Errors);

        var fileToDownload = filesToDownloadResult.FilesToDownload.SingleOrDefault();
        if (fileToDownload is null)
        {
            var errors = new[] { "Unable to get the single file historical prices file for " +
                                 $"{RegionalTransmissionOperator} {priceIndex.PriceMarketId} {startDate:yyyy} {startDate:MMMM}" };
            return new DownloadHistoricalPricesResult(false, errors);
        }

        _logger.LogInformation("Downloading {rto} {priceMarketId} {startDateYear} {startDateMonth} prices",
        RegionalTransmissionOperator, priceIndex.PriceMarketId, $"{startDate:yyyy}", $"{startDate:MMMM}");

        var result = await DownloadPricesFileAsync(fileToDownload, retryAttempts, delayInSecondsBetweenRetryAttempts, cancellationToken);
        return !result.Success
            ? new DownloadHistoricalPricesResult(false, result.Errors)
            : new DownloadHistoricalPricesResult(true, Array.Empty<string>());
    }
}