using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Prices.AzureBlobStorage;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Downloader.Services.Services;

namespace Prices.Downloader.Tests.Mocks
{
    public class MockPricesFileDownloaderFactory : IPricesFileDownloaderFactory
    {
        private readonly IReadOnlyDictionary<Rtos, IPricesFileDownloader> _pricesFileDownloaders;

        public MockPricesFileDownloaderFactory(IAzureBlobStorageClientFactory factory, ILoggerFactory loggerFactory, IOptions<Settings> settings, IClock clock)
        {
            var dbContextFactory = new MockDbContextFactory();

            var pricesFileProcessorFactory = new MockPricesFileProcessorFactory(loggerFactory, clock);

            var caisoPricesFileDownloaderLogger = loggerFactory.CreateLogger<CaisoPricesFileDownloader>();
            var ercotPricesFileDownloaderLogger = loggerFactory.CreateLogger<ErcotPricesFileDownloader>();

            _pricesFileDownloaders = new Dictionary<Rtos, IPricesFileDownloader>
            {
                { Rtos.CAISO, new CaisoPricesFileDownloader(factory, dbContextFactory, pricesFileProcessorFactory, caisoPricesFileDownloaderLogger, settings) },
                { Rtos.ERCOT, new ErcotPricesFileDownloader(factory, dbContextFactory, pricesFileProcessorFactory, ercotPricesFileDownloaderLogger, settings) },
            };
        }

        public IPricesFileDownloader? GetDownloaderByRto(Rtos rto) => _pricesFileDownloaders.GetValueOrDefault(rto);
    }
}
