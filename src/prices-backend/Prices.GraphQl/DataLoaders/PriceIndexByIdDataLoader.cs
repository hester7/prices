using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PriceIndexByIdDataLoader : BatchDataLoader<PriceIndexes, PriceIndex>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PriceIndexByIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<PriceIndexes, PriceIndex>> LoadBatchAsync(IReadOnlyList<PriceIndexes> ids, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PriceIndexes.AsNoTracking().Where(t => ids.Contains(t.Id)).ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}