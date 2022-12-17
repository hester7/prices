using Prices.Core.Application.Models;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Interfaces.Services
{
    public interface IPricesFileProcessor : IServiceByRto
    {
        public FileFormats FileFormat { get; }
        FileProcessorResult ProcessPrices(Stream blob, Rtos regionalTransmissionOperatorId, PriceMarkets priceMarketId,
            PriceIndexes priceIndexId, PriceTypes priceTypeId, IEnumerable<PricingNode> pricingNodes);
    }
}