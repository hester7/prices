using Prices.Core.Application.Interfaces.Services;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Interfaces.Factories;

public interface IPricesFileProcessorFactory
{
    IPricesFileProcessor? GetProcessorByRtoAndFileFormat(Rtos rto, FileFormats fileFormat);
}