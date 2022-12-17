using NodaTime;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models
{
    public class Price
    {
        public PriceIndexes PriceIndexId { get; set; }
        public int PricingNodeId { get; set; }
        public Instant IntervalStartTimeUtc { get; set; }
        public Instant IntervalEndTimeUtc { get; set; }
        public int IntervalLength { get; set; }
        public decimal LmpPrice { get; set; }
        public decimal EnergyPrice { get; set; }
        public decimal CongestionPrice { get; set; }
        public decimal LossPrice { get; set; }
        public string PricingNodeName { get; set; } = null!;
        public Instant LastModifiedAtUtc { get; set; }
    }
}
