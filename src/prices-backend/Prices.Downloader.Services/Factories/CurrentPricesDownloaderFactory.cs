using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;

namespace Prices.Downloader.Services.Factories;

public class CurrentPricesDownloaderFactory : ServiceByRtoFactory<ICurrentPricesDownloader>, ICurrentPricesDownloaderFactory
{
    public CurrentPricesDownloaderFactory(IEnumerable<ICurrentPricesDownloader> services) : base(services)
    {
    }
}