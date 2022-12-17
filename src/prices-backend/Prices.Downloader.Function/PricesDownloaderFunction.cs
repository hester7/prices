using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;
using Prices.Persistence.EntityFramework.Extensions;
using Serilog;

namespace Prices.Downloader.Function
{
    public class PricesDownloaderFunction
    {
        private readonly IDbContextFactory<PricesContext> _contextFactory;
        private readonly IPricingNodesDownloaderFactory _pricingNodesDownloaderFactory;
        private readonly IHistoricalPricesFileDownloaderFactory _historicalPricesFileDownloaderFactory;
        private readonly IPricesFileDownloaderFactory _pricesFileDownloaderFactory;
        private readonly ILogger _logger;

        public PricesDownloaderFunction(
            IDbContextFactory<PricesContext> contextFactory,
            IPricingNodesDownloaderFactory pricingNodesDownloaderFactory,
            IHistoricalPricesFileDownloaderFactory historicalPricesFileDownloaderFactory,
            IPricesFileDownloaderFactory pricesFileDownloaderFactory,
            ILogger logger)
        {
            _contextFactory = contextFactory;
            _pricingNodesDownloaderFactory = pricingNodesDownloaderFactory;
            _historicalPricesFileDownloaderFactory = historicalPricesFileDownloaderFactory;
            _pricesFileDownloaderFactory = pricesFileDownloaderFactory;
            _logger = logger;
        }

        // 0 * * * * *  every minute

        // At minute 0 past hour 0 and every hour from 14 through 23.
        [FunctionName("DownloadDayAheadPrices")]
        public async Task DownloadDayAheadPrices([TimerTrigger("0 0,14-23 * * *", UseMonitor = true)] TimerInfo myTimer,
            CancellationToken cancellationToken)
        {
            _logger.Information("Download Day-Ahead Prices trigger function executed");

            const PriceMarkets priceMarketId = PriceMarkets.DAM;
            await DownloadDailyPricesForMarketAsync(priceMarketId, cancellationToken);
        }

        // At every 5th minute
        [FunctionName("DownloadRealTimePrices")]
        public async Task DownloadRealTimePrices([TimerTrigger("*/5 * * * *", UseMonitor = true)] TimerInfo myTimer, CancellationToken cancellationToken)
        {
            _logger.Information("Download Real-Time Prices trigger function executed");

            const PriceMarkets priceMarketId = PriceMarkets.RTM;
            await DownloadDailyPricesForMarketAsync(priceMarketId, cancellationToken);
        }

        // TODO: for now, do not download fifteen minute market prices
        //// At every 15th minute
        //[FunctionName("DownloadFifteenMinutePrices")]
        //public async Task DownloadFifteenMinutePrices([TimerTrigger("*/15 * * * *", UseMonitor = true)] TimerInfo myTimer,
        //    CancellationToken cancellationToken)
        //{
        //    _logger.Information("Download Fifteen-Minute Prices trigger function executed");

        //    const PriceMarkets priceMarketId = PriceMarkets.FMM;
        //    await DownloadDailyPricesForMarketAsync(priceMarketId, cancellationToken);
        //}

        // Every 12 hours
        [FunctionName("DownloadHistoricalPrices")]
        public async Task DownloadHistoricalPrices([TimerTrigger("0 */12 * * *", UseMonitor = true)] TimerInfo myTimer, CancellationToken cancellationToken)
        {
            _logger.Information("Download Historical Prices trigger function executed");

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            // Download pricing nodes first
            var rtos = await context.RegionalTransmissionOperators.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            // TODO: for now, do not download pricing nodes
            //var tasks = rtos.Select(rto => DownloadPricingNodesAsync(rto.RegionalTransmissionOperatorId, cancellationToken));
            //await Task.WhenAll(tasks);

            // Download historical prices
            var tasks = rtos.Select(rto => DownloadHistoricalPricesAsync(rto.Id, cancellationToken));
            await Task.WhenAll(tasks);
        }

        private async Task DownloadPricingNodesAsync(Rtos rto, CancellationToken cancellationToken)
        {
            var pricingNodesDownloader = _pricingNodesDownloaderFactory.GetDownloaderByRto(rto);
            if (pricingNodesDownloader is null)
            {
                _logger.Warning("No pricing nodes downloader implemented for {rto}", rto);
                return;
            }

            _logger.Information("Downloading {rto} pricing nodes", rto);

            var result = await pricingNodesDownloader.DownloadPricingNodesAsync(cancellationToken: cancellationToken);
            if (!result.Success)
            {
                _logger.Error("Error downloading {rto} pricing nodes", rto);
                result.Errors.ToList().ForEach(e => _logger.Error(e));
                return;
            }

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await context.PricingNodes.UpsertRangeAsync(result.PricingNodes, cancellationToken);
            var entries = await context.SaveChangesAsync(cancellationToken);
            _logger.Information("Saved {entries} pricing nodes", entries);
        }

        private async Task DownloadHistoricalPricesAsync(Rtos rto, CancellationToken cancellationToken = default)
        {
            var historicalPricesFileDownloader = _historicalPricesFileDownloaderFactory.GetDownloaderByRto(rto);
            if (historicalPricesFileDownloader is null)
            {
                _logger.Warning("No historical prices downloader implemented for {rto}.", rto);
                return;
            }

            // Download the month of the previous day
            var previousDay = DateTime.Today.AddDays(-1);
            var month = new DateTime(previousDay.Year, previousDay.Month, 1);
            await historicalPricesFileDownloader.DownloadHistoricalPricesAsync(month.Year, month.Month, cancellationToken: cancellationToken);
        }

        private async Task DownloadDailyPricesForMarketAsync(PriceMarkets priceMarketId, CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var priceIndexes = await context.PriceIndexes.Where(i => i.PriceMarketId == priceMarketId).AsNoTracking().ToListAsync(cancellationToken);
            var tasks = priceIndexes.Select(priceIndex => DownloadDailyPricesForIndexAsync(priceIndex, cancellationToken));
            await Task.WhenAll(tasks);
        }

        private async Task DownloadDailyPricesForIndexAsync(PriceIndex priceIndex, CancellationToken cancellationToken)
        {
            var pricesDownloader = _pricesFileDownloaderFactory.GetDownloaderByRto(priceIndex.RegionalTransmissionOperatorId);
            if (pricesDownloader is null)
            {
                _logger.Warning("No prices file downloader implemented for {rto}", priceIndex.RegionalTransmissionOperatorId);
                return;
            }

            var (startDate, endDate) = pricesDownloader.GetDailyPricesDownloadDates(priceIndex.PriceMarketId);

            var filesToDownloadResult = await pricesDownloader.GetFilesToDownload(PriceTypes.Daily, priceIndex, pricesDownloader.DailyPricesFileFormat,
                startDate, endDate, cancellationToken: cancellationToken);
            if (!filesToDownloadResult.Success)
            {
                _logger.Error("Error getting files to downloader for {rto} {priceMarketId} {startDate} - {endDate}",
                    priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId, $"{startDate:d}", $"{endDate:d}");
                filesToDownloadResult.Errors.ToList().ForEach(e => _logger.Error(e));
                return;
            }

            var tasks = filesToDownloadResult.FilesToDownload
                .Take(5) // Limit to 5
                .Select(fileToDownload => DownloadDailyPricesForIndexAsync(pricesDownloader, fileToDownload, priceIndex, startDate, endDate, cancellationToken))
                .ToList();

            await Task.WhenAll(tasks);
        }

        private async Task DownloadDailyPricesForIndexAsync(IPricesFileDownloader pricesDownloader, PricesFileToDownload fileToDownload, PriceIndex priceIndex,
            LocalDate startDate, LocalDate endDate, CancellationToken cancellationToken)
        {
            _logger.Information("Downloading {rto} {priceMarketId} {startDate} - {endDate} from {requestUri}",
                priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId, $"{startDate:d}", $"{endDate:d}", fileToDownload.RequestUri);

            // Build in slight delay to avoid deadlock issues
            await Task.Delay(Random.Shared.Next(100, 1000), cancellationToken);

            var result = await pricesDownloader.DownloadPricesFileAsync(fileToDownload, cancellationToken: cancellationToken);
            if (!result.Success)
            {
                _logger.Error("Error downloading {rto} {priceMarketId} {startDate} - {endDate} prices",
                    priceIndex.RegionalTransmissionOperatorId, priceIndex.PriceMarketId, $"{startDate:d}", $"{endDate:d}");
                result.Errors.ToList().ForEach(e => _logger.Error(e));
                return;
            }

            _logger.Information("Uploaded {fileName} prices", result.Metadata!.FileName);
        }
    }
}
