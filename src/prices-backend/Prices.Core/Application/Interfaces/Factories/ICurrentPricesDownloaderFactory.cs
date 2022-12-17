using Prices.Core.Application.Interfaces.Services;

namespace Prices.Core.Application.Interfaces.Factories;

public interface ICurrentPricesDownloaderFactory : IServiceByRtoFactory<ICurrentPricesDownloader>
{
}