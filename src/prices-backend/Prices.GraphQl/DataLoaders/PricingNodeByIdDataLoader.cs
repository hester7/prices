using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PricingNodeByIdDataLoader : BatchDataLoader<int, PricingNode>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PricingNodeByIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<int, PricingNode>> LoadBatchAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.PricingNodes.AsNoTracking().Where(t => ids.Contains(t.Id)).ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}