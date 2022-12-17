using Microsoft.Extensions.DependencyInjection;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Downloader.Services.Factories;
using Prices.Downloader.Services.Services;

namespace Prices.Downloader.Services
{
    public static class PricesDownloaderServicesRegistration
    {
        public static IServiceCollection AddPricesDownloaderServices(this IServiceCollection services, Settings settings) =>
            services
                .Configure<Settings>(options => { options.SasUri = settings.SasUri; })
                .AddSingleton<IPricesFileDownloaderFactory, PricesFileDownloaderFactory>()
                .AddSingleton<IPricesFileProcessorFactory, PricesFileProcessorFactory>()
                .AddSingleton<IPricingNodesDownloaderFactory, PricingNodesDownloaderFactory>()
                .AddSingleton<ICurrentPricesDownloaderFactory, CurrentPricesDownloaderFactory>()
                .AddSingleton<IHistoricalPricesFileDownloaderFactory, HistoricalPricesFileDownloaderFactory>()

                .AddSingleton<IPricesFileDownloader, CaisoPricesFileDownloader>()
                .AddSingleton<IPricesFileProcessor, CaisoPricesXmlFileProcessor>()
                .AddSingleton<IPricingNodesDownloader, CaisoPricingNodesDownloader>()
                .AddSingleton<ICurrentPricesDownloader, CaisoCurrentPricesDownloader>()
                .AddSingleton<IHistoricalPricesFileDownloader, CaisoHistoricalPricesFileDownloader>()

                .AddSingleton<IPricesFileDownloader, ErcotPricesFileDownloader>()
                .AddSingleton<IPricesFileProcessor, ErcotPricesExcelFileProcessor>()
                .AddSingleton<IPricesFileProcessor, ErcotPricesXmlFileProcessor>()
                .AddSingleton<ICurrentPricesDownloader, ErcotCurrentPricesDownloader>()
                .AddSingleton<IHistoricalPricesFileDownloader, ErcotHistoricalPricesFileDownloader>()
            ;
    }
}