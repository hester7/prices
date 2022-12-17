using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class PricingNodeTypesQuery
{
    public IQueryable<PricingNodeType> GetPricingNodeTypes(PricesContext context) => context.PricingNodeTypes;

    public async Task<PricingNodeType?> GetPricingNodeTypeById(
        PricingNodeTypes id,
        PricingNodeTypeByIdDataLoader pricingNodeTypeById,
        CancellationToken cancellationToken)
        => await pricingNodeTypeById.LoadAsync(id, cancellationToken);

    public async Task<IEnumerable<PricingNodeType?>> GetPricingNodeTypesById(
        PricingNodeTypes[] ids,
        PricingNodeTypeByIdDataLoader pricingNodeTypeById,
        CancellationToken cancellationToken)
        => await pricingNodeTypeById.LoadAsync(ids, cancellationToken);
}