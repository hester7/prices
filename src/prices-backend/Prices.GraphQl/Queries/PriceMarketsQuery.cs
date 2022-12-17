using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class PriceMarketsQuery
{
    public IQueryable<PriceMarket> GetPriceMarkets(PricesContext context) => context.PriceMarkets;

    public async Task<PriceMarket?> GetPriceMarketById(
        PriceMarkets id,
        PriceMarketByIdDataLoader priceMarketById,
        CancellationToken cancellationToken)
        => await priceMarketById.LoadAsync(id, cancellationToken);

    public async Task<IEnumerable<PriceMarket?>> GetPriceMarketsById(
        PriceMarkets[] ids,
        PriceMarketByIdDataLoader priceMarketById,
        CancellationToken cancellationToken)
        => await priceMarketById.LoadAsync(ids, cancellationToken);
}