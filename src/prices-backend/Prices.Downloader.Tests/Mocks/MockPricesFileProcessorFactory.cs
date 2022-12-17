using Microsoft.Extensions.Logging;
using NodaTime;
using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;
using Prices.Downloader.Services.Services;

namespace Prices.Downloader.Tests.Mocks;

public class MockPricesFileProcessorFactory : IPricesFileProcessorFactory
{
    private readonly IReadOnlyDictionary<(Rtos, FileFormats), IPricesFileProcessor> _pricesFileProcessors;

    public MockPricesFileProcessorFactory(ILoggerFactory loggerFactory, IClock clock)
    {
        var caisoPricesXmlFileProcessorLogger = loggerFactory.CreateLogger<CaisoPricesXmlFileProcessor>();
        var ercotPricesXmlFileProcessorLogger = loggerFactory.CreateLogger<ErcotPricesXmlFileProcessor>();
        var ercotPricesExcelFileProcessorLogger = loggerFactory.CreateLogger<ErcotPricesExcelFileProcessor>();

        _pricesFileProcessors = new Dictionary<(Rtos, FileFormats), IPricesFileProcessor>
        {
            { (Rtos.CAISO, FileFormats.XML), new CaisoPricesXmlFileProcessor(clock, caisoPricesXmlFileProcessorLogger) },
            { (Rtos.ERCOT, FileFormats.XML), new ErcotPricesXmlFileProcessor(clock, ercotPricesXmlFileProcessorLogger) },
            { (Rtos.ERCOT, FileFormats.Excel), new ErcotPricesExcelFileProcessor(clock, ercotPricesExcelFileProcessorLogger) },
        };
    }

    public IPricesFileProcessor? GetProcessorByRtoAndFileFormat(Rtos rto, FileFormats fileFormat) => _pricesFileProcessors.GetValueOrDefault((rto, fileFormat));
}