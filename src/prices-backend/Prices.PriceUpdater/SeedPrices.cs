using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Domain.Enums;
using Prices.Persistence.EntityFramework;

namespace Prices.PriceUpdater;

public class SeedPrices : ISeedPrices
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;
    private readonly IHistoricalPricesFileDownloaderFactory _historicalPricesFileDownloaderFactory;
    private readonly ILogger<SeedPrices> _logger;

    public SeedPrices(
        IDbContextFactory<PricesContext> contextFactory,
        IHistoricalPricesFileDownloaderFactory historicalPricesFileDownloaderFactory,
        ILogger<SeedPrices> logger)
    {
        _contextFactory = contextFactory;
        _historicalPricesFileDownloaderFactory = historicalPricesFileDownloaderFactory;
        _logger = logger;
    }

    public async Task Run(int startYear, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var rtos = await context.RegionalTransmissionOperators.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        var tasks = rtos.Select(rto => DownloadHistoricalPricesAsync(rto.Id, startYear, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task DownloadHistoricalPricesAsync(Rtos rto, int startYear, CancellationToken cancellationToken = default)
    {
        try
        {
            var historicalPricesFileDownloader = _historicalPricesFileDownloaderFactory.GetDownloaderByRto(rto);
            if (historicalPricesFileDownloader is null)
            {
                _logger.LogWarning("No historical prices downloader implemented for {rto}.", rto);
                return;
            }

            var currentYear = DateTime.Today.Year;
            for (var year = startYear; year <= currentYear; year++)
            {
                _logger.LogInformation("Downloading {rto} {year} prices", rto, year);
                await historicalPricesFileDownloader.DownloadHistoricalPricesAsync(year, null, cancellationToken: cancellationToken);
                _logger.LogInformation("Downloaded {rto} {year} prices and uploaded to Azure", rto, year);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}