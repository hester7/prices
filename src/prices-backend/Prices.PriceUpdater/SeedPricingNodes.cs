using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Domain.Enums;
using Prices.Persistence.EntityFramework;
using Prices.Persistence.EntityFramework.Extensions;

namespace Prices.PriceUpdater;

public class SeedPricingNodes : ISeedPricingNodes
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;
    private readonly IPricingNodesDownloaderFactory _pricingNodesDownloaderFactory;
    private readonly ILogger<SeedPricingNodes> _logger;

    public SeedPricingNodes(IDbContextFactory<PricesContext> contextFactory, IPricingNodesDownloaderFactory pricingNodesDownloaderFactory,
        ILogger<SeedPricingNodes> logger)
    {
        _contextFactory = contextFactory;
        _pricingNodesDownloaderFactory = pricingNodesDownloaderFactory;
        _logger = logger;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var rtos = await context.RegionalTransmissionOperators.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        var tasks = rtos.Select(rto => DownloadPricingNodesAsync(rto.Id, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task DownloadPricingNodesAsync(Rtos rto, CancellationToken cancellationToken = default)
    {
        try
        {
            var pricingNodesDownloader = _pricingNodesDownloaderFactory.GetDownloaderByRto(rto);
            if (pricingNodesDownloader is null)
            {
                _logger.LogWarning("No pricing nodes downloader implemented for {rto}.", rto);
                return;
            }

            var result = await pricingNodesDownloader.DownloadPricingNodesAsync(cancellationToken: cancellationToken);
            if (!result.Success)
            {
                _logger.LogError("Error downloading {rto} pricing nodes", rto);
                result.Errors.ToList().ForEach(e => _logger.LogError(e));
                return;
            }

            await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await context.PricingNodes.UpsertRangeAsync(result.PricingNodes, cancellationToken);
            var entries = await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved {entries} pricing nodes.", entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}