using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Interfaces.Services;

public interface IServiceByRto
{
    public Rtos RegionalTransmissionOperator { get; }
}