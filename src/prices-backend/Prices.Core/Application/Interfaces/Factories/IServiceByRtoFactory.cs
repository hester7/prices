using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Interfaces.Factories;

public interface IServiceByRtoFactory<out T> where T : IServiceByRto
{
    T? GetDownloaderByRto(Rtos rto);
}