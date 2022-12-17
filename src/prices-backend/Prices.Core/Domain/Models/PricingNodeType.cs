using Prices.Core.Domain.Enums;

namespace Prices.Core.Domain.Models;

public class PricingNodeType
{
    public PricingNodeType()
    {
        PricingNodes = new HashSet<PricingNode>();
    }

    public PricingNodeTypes Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<PricingNode> PricingNodes { get; set; }
}