using Prices.Core.Application.Interfaces.Factories;
using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;

namespace Prices.Downloader.Services.Factories;

public class ServiceByRtoFactory<T> : IServiceByRtoFactory<T> where T: IServiceByRto
{
    private readonly IReadOnlyDictionary<Rtos, T> _services;

    public ServiceByRtoFactory(IEnumerable<T> services)
    {
        _services = services
            .GroupBy(x => x.RegionalTransmissionOperator)
            .ToDictionary(x => x.Key, x => x.Last());
    }

    public T? GetDownloaderByRto(Rtos rto) => _services.GetValueOrDefault(rto);
}