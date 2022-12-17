using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PriceMarketByIdDataLoader : BatchDataLoader<PriceMarkets, PriceMarket>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PriceMarketByIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<PriceMarkets, PriceMarket>> LoadBatchAsync(IReadOnlyList<PriceMarkets> ids, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PriceMarkets.AsNoTracking().Where(t => ids.Contains(t.Id)).ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}