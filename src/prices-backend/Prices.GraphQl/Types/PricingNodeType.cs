using Prices.Core.Application.Models;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;

namespace Prices.GraphQl.Types;

[ExtendObjectType(typeof(PricingNode),
    IgnoreProperties = new[]
    {
        nameof(PricingNode.DisplayName),
        nameof(PricingNode.PricingNodeName),
        nameof(PricingNode.RegionalTransmissionOperator),
    })]
public sealed class PricingNodeType
{
    [BindMember(nameof(PricingNode.Name))]
    public string GetName([Parent] PricingNode pricingNode) => pricingNode.PricingNodeName;

    public async Task<IEnumerable<PriceByPricingNode>> GetPricesAsync(
        ChangeSpan span,
        [Parent] PricingNode parent,
        PriceByPricingNodeIdDataLoader priceByPricingNodeId,
        CancellationToken cancellationToken)
    {
        return await priceByPricingNodeId.LoadAsync(new PricingNodeIdAndSpan(parent.Id, span), cancellationToken);
    }
}