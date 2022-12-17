using Prices.Core.Application.Models;

namespace Prices.Core.Application.Interfaces.Services;

public interface ICurrentPricesDownloader : IServiceByRto
{
    Task<FileProcessorResult> DownloadCurrentPricesAsync(IEnumerable<string>? nodes = null, int retryAttempts = 2,
        int delayInSecondsBetweenRetryAttempts = 30, CancellationToken cancellationToken = default);
}