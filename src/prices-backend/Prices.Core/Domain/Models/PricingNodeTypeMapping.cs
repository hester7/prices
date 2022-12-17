using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class PricingNodeTypeMapping
    {
        public PricingNodeTypes PricingNodeTypeId { get; set; }
        public Rtos RegionalTransmissionOperatorId { get; set; }
        public string Code { get; set; } = null!;
    }
}
