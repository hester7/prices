using Prices.Core.Application.Models;

namespace Prices.Core.Application.Interfaces.Services;

public interface IHistoricalPricesFileDownloader : IServiceByRto
{
    Task<DownloadHistoricalPricesResult> DownloadHistoricalPricesAsync(int year, int? month, IEnumerable<string>? nodes = null, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default);
}