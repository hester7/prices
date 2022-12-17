using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class PriceIndexesQuery
{
    public IQueryable<PriceIndex> GetPriceIndexes(PricesContext context) => context.PriceIndexes;
    
    public async Task<PriceIndex?> GetPriceIndexById(
        PriceIndexes id,
        PriceIndexByIdDataLoader priceIndexById,
        CancellationToken cancellationToken)
        => await priceIndexById.LoadAsync(id, cancellationToken);

    public async Task<IEnumerable<PriceIndex?>> GetPriceIndexesById(
        PriceIndexes[] ids,
        PriceIndexByIdDataLoader priceIndexById,
        CancellationToken cancellationToken)
        => await priceIndexById.LoadAsync(ids, cancellationToken);
}