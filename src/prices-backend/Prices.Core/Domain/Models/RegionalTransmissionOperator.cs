using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class RegionalTransmissionOperator
    {
        public RegionalTransmissionOperator()
        {
            PriceIndexes = new HashSet<PriceIndex>();
            PricingNodes = new HashSet<PricingNode>();
        }

        public Rtos Id { get; set; }
        public string Name { get; set; } = null!;
        public string LegalName { get; set; } = null!;

        public ICollection<PriceIndex> PriceIndexes { get; set; }
        public ICollection<PricingNode> PricingNodes { get; set; }
    }
}
