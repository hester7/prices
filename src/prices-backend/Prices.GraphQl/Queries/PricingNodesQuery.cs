using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;
using Prices.GraphQl.Types;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class PricingNodesQuery
{
    [UseFiltering(typeof(PricingNodeFilterInputType))]
    [UseSorting(typeof(PricingNodeSortInputType))]
    public IQueryable<PricingNode> GetPricingNodes(PricesContext context) => context.PricingNodes;

    public async Task<PricingNode?> GetPricingNodeByRtoAndName(
        Rtos rto,
        string name,
        PricingNodeByNameDataLoader pricingNodeByName,
        CancellationToken cancellationToken)
        => await pricingNodeByName.LoadAsync(new RtoIdAndPricingNodeName(rto, name), cancellationToken);

    [NodeResolver]
    public async Task<PricingNode?> GetPricingNodeById(
        int id,
        PricingNodeByIdDataLoader pricingNodeById,
        CancellationToken cancellationToken)
        => await pricingNodeById.LoadAsync(id, cancellationToken);

    public async Task<IEnumerable<PricingNode?>> GetPricingNodesById(
        [ID(nameof(PricingNode))] int[] ids,
        PricingNodeByIdDataLoader pricingNodeById,
        CancellationToken cancellationToken)
        => await pricingNodeById.LoadAsync(ids, cancellationToken);
}