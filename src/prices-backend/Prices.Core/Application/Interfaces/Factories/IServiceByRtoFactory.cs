using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Interfaces.Factories;

public interface IServiceByRtoFactory<out T>
{
    T? GetDownloaderByRto(Rtos rto);
}