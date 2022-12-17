namespace Prices.GraphQl.Types;

[ExtendObjectType(typeof(Core.Domain.Models.PricingNodeType), IgnoreProperties = new[] { nameof(Core.Domain.Models.PricingNodeType.PricingNodes) })]
public sealed class PricingNodeTypeType
{
}