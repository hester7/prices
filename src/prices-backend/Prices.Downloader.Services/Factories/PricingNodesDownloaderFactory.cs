using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;

namespace Prices.Downloader.Services.Factories
{
    public class PricingNodesDownloaderFactory : ServiceByRtoFactory<IPricingNodesDownloader>, IPricingNodesDownloaderFactory
    {
        public PricingNodesDownloaderFactory(IEnumerable<IPricingNodesDownloader> services) : base(services)
        {
        }
    }
}
