using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;

namespace Prices.Downloader.Services.Factories;

public class PricesFileProcessorFactory : IPricesFileProcessorFactory
{
    private readonly IReadOnlyDictionary<(Rtos, FileFormats), IPricesFileProcessor> _pricesFileProcessors;

    public PricesFileProcessorFactory(IEnumerable<IPricesFileProcessor> pricesFileProcessors)
    {
        _pricesFileProcessors = pricesFileProcessors
            .GroupBy(x => (x.RegionalTransmissionOperator, x.FileFormat))
            .ToDictionary(x => x.Key, x => x.Last());
    }

    public IPricesFileProcessor? GetProcessorByRtoAndFileFormat(Rtos rto, FileFormats fileFormat) => _pricesFileProcessors.GetValueOrDefault((rto, fileFormat));
}