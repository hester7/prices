using NodaTime;
using Prices.Core.Domain.Enums;
using System.Text.Json.Serialization;

namespace Prices.Core.Domain.Models
{
    public class PricingNode
    {
        public int Id { get; set; }
        public Rtos RegionalTransmissionOperatorId { get; set; }
        public string Name { get; set; } = null!;
        public string? DisplayName { get; set; }
        public PricingNodeTypes PricingNodeTypeId { get; set; }
        public Instant? StartDateUtc { get; set; }
        public Instant? EndDateUtc { get; set; }
        public decimal? CurrentPrice { get; set; }
        public decimal? Change24Hour { get; set; }
        public Instant CreatedAtUtc { get; set; }
        public Instant LastModifiedAtUtc { get; set; }

        public PricingNodeType PricingNodeType { get; set; } = null!;
        public RegionalTransmissionOperator RegionalTransmissionOperator { get; set; } = null!;

        [JsonIgnore]
        public string PricingNodeName => DisplayName ?? Name;
    }
}
