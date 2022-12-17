using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;

namespace Prices.Downloader.Services.Factories;

public class HistoricalPricesFileDownloaderFactory : ServiceByRtoFactory<IHistoricalPricesFileDownloader>, IHistoricalPricesFileDownloaderFactory
{
    public HistoricalPricesFileDownloaderFactory(IEnumerable<IHistoricalPricesFileDownloader> services) : base(services)
    {
    }
}