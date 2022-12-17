using HotChocolate.Data.Filters;
using Prices.Core.Domain.Models;

namespace Prices.GraphQl.Types;

public sealed class PricingNodeFilterInputType : FilterInputType<PricingNode>
{
    protected override void Configure(IFilterInputTypeDescriptor<PricingNode> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.RegionalTransmissionOperatorId);
        descriptor.Field(t => t.PricingNodeName);
        descriptor.Field(t => t.PricingNodeTypeId);
        descriptor.Field(t => t.CurrentPrice);
    }
}