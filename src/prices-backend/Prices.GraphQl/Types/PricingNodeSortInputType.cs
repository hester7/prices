using HotChocolate.Data.Sorting;
using Prices.Core.Domain.Models;

namespace Prices.GraphQl.Types;

public sealed class PricingNodeSortInputType : SortInputType<PricingNode>
{
    protected override void Configure(ISortInputTypeDescriptor<PricingNode> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.RegionalTransmissionOperatorId);
        descriptor.Field(t => t.PricingNodeName);
        descriptor.Field(t => t.PricingNodeTypeId);
        descriptor.Field(t => t.CurrentPrice);
    }
}