using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl;
using Prices.Persistence.EntityFramework;

namespace Prices.PriceUpdater
{
    public sealed class PriceUpdaterService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly IDbContextFactory<PricesContext> _contextFactory;
        private readonly ISeedPricingNodes _seedPricingNodes;
        private readonly ISeedPrices _seedPrices;
        private readonly ICurrentPricesDownloaderFactory _currentPricesDownloaderFactory;
        private readonly ITopicEventSender _topicEventSender;
        private readonly ILogger<PriceUpdaterService> _logger;
        private readonly IClock _clock;
        private bool _disposed;

        public PriceUpdaterService(
            IDbContextFactory<PricesContext> contextFactory,
            ISeedPricingNodes seedPricingNodes,
            ISeedPrices seedPrices,
            ICurrentPricesDownloaderFactory currentPricesDownloaderFactory,
            ITopicEventSender topicEventSender,
            ILogger<PriceUpdaterService> logger,
            IClock clock)
        {
            _contextFactory = contextFactory;
            _seedPricingNodes = seedPricingNodes;
            _seedPrices = seedPrices;
            _currentPricesDownloaderFactory = currentPricesDownloaderFactory;
            _topicEventSender = topicEventSender;
            _logger = logger;
            _clock = clock;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            SeedDatabase();
            BeginUpdatePrices();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
                _cts.Dispose();
                _disposed = true;
            }
        }

        private void SeedDatabase()
            => Task.Factory.StartNew(
                async () =>
                {
                    // TODO: for now, do not download pricing nodes
                    //await SeedPricingNodesAsync(_cts.Token);
                    await SeedPricesAsync(_cts.Token);
                },
                _cts.Token);

        private void BeginUpdatePrices()
            => Task.Factory.StartNew(
                async () =>
                {
                    await UpdatePricesAsync(_cts.Token);
                },
                _cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

        private async Task SeedPricingNodesAsync(CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var pricingNodes = await context.PricingNodes.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
            if (pricingNodes.Count > SeedData.PricingNodes.Length)
                return;

            await _seedPricingNodes.Run(cancellationToken: cancellationToken);
        }

        private async Task SeedPricesAsync(CancellationToken cancellationToken)
        {
            var startYear = DateTime.Today.Year - 1;
            await _seedPrices.Run(startYear, cancellationToken: cancellationToken);
        }

        private async Task UpdatePricesAsync(CancellationToken cancellationToken)
        {
            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var rtos = await context.RegionalTransmissionOperators.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tasks = rtos.Select(rto => UpdatePricesAsync(rto, cancellationToken));
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    // if there is an error we will retry
                    _logger.LogError("An error occurred while updating prices: {exMessage}", ex.Message);
                }

                //await Task.Delay(Random.Shared.Next(10_000, 20_000), cancellationToken);
                await Task.Delay(5_000, cancellationToken);
            }
        }

        private async Task UpdatePricesAsync(RegionalTransmissionOperator rto, CancellationToken cancellationToken)
        {
            var downloader = _currentPricesDownloaderFactory.GetDownloaderByRto(rto.Id);
            if (downloader is null)
            {
                _logger.LogInformation("No current prices downloader implemented for {rto}.", rto.Id);
                return;
            }

            var result = await downloader.DownloadCurrentPricesAsync(cancellationToken: cancellationToken);
            if (!result.Success)
            {
                result.Errors.ToList().ForEach(e => _logger.LogError(e));
                return;
            }

            var currentPrices = result.Prices.ToList();
            if (!currentPrices.Any())
            {
                _logger.LogWarning("No current prices downloaded for {rto}.", rto.Id);
                return;
            }

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            var realTimePriceIndexId = await context.PriceIndexes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.RegionalTransmissionOperatorId == rto.Id && i.PriceMarketId == PriceMarkets.RTM,
                    cancellationToken: cancellationToken);

            var priceIndex = realTimePriceIndexId?.Id;
            var intervalEndTimeUtc = currentPrices[0].IntervalEndTimeUtc.Minus(Duration.FromMinutes(1440)).RoundUp(TimeSpan.FromMinutes(15));
            var prices24HoursAgo = await context.Prices
                .Where(p => p.PriceIndexId == priceIndex && p.IntervalEndTimeUtc == intervalEndTimeUtc)
                .AsNoTracking()
                .ToDictionaryAsync(p => p.PricingNodeId, cancellationToken);

            var pricingNodes = await context.PricingNodes.ToDictionaryAsync(pn => pn.Id, pn => pn, cancellationToken: cancellationToken);

            var updateCount = 0;
            foreach (var currentPrice in currentPrices)
            {
                pricingNodes.TryGetValue(currentPrice.PricingNodeId, out var pricingNode);
                if (pricingNode is null)
                    continue;

                prices24HoursAgo.TryGetValue(currentPrice.PricingNodeId, out var price24HoursAgo);
                var updated = await UpdatePriceAsync(pricingNode, currentPrice, price24HoursAgo, context, cancellationToken);

                if (updated)
                    updateCount++;
            }

            if (updateCount > 0)
                _logger.LogInformation("Updated {updateCount} current prices.", updateCount);
        }

        private async Task<bool> UpdatePriceAsync(PricingNode pricingNode, Price currentPrice, Price? price24HoursAgo, PricesContext context,
            CancellationToken cancellationToken)
        {
            if (pricingNode.CurrentPrice == currentPrice.LmpPrice)
            {
                return false;
            }

            pricingNode.LastModifiedAtUtc = _clock.GetCurrentInstant();
            pricingNode.CurrentPrice = currentPrice.LmpPrice;
            pricingNode.Change24Hour = price24HoursAgo is not null ? currentPrice.LmpPrice - price24HoursAgo.LmpPrice : null;

            await context.SaveChangesAsync(cancellationToken);

            // Delay so it appears they are changing at different times
            await Task.Delay(Random.Shared.Next(200, 400), cancellationToken);
            await _topicEventSender.SendAsync(Constants.OnCurrentPriceChange, pricingNode.Id, cancellationToken);

            return true;
        }
    }
}
