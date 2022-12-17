using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class PriceIndex
    {
        public PriceIndex()
        {
            PricesFiles = new HashSet<PricesFile>();
        }

        public PriceIndexes Id { get; set; }
        public string Name { get; set; } = null!;
        public Rtos RegionalTransmissionOperatorId { get; set; }
        public PriceMarkets PriceMarketId { get; set; }

        public PriceMarket PriceMarket { get; set; } = null!;
        public RegionalTransmissionOperator RegionalTransmissionOperator { get; set; } = null!;
        public ICollection<PricesFile> PricesFiles { get; set; }
    }
}
