using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;

namespace Prices.Downloader.Services.Factories;

public class PricesFileDownloaderFactory : ServiceByRtoFactory<IPricesFileDownloader>, IPricesFileDownloaderFactory
{
    public PricesFileDownloaderFactory(IEnumerable<IPricesFileDownloader> services) : base(services)
    {
    }
}