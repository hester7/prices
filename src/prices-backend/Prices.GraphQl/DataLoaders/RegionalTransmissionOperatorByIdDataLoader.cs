using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class RegionalTransmissionOperatorByIdDataLoader : BatchDataLoader<Rtos, RegionalTransmissionOperator>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public RegionalTransmissionOperatorByIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<Rtos, RegionalTransmissionOperator>> LoadBatchAsync(IReadOnlyList<Rtos> ids, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.RegionalTransmissionOperators
            .Include(r => r.PricingNodes)
            .Include(r => r.PriceIndexes)
            .Where(t => ids.Contains(t.Id))
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Id, cancellationToken);
    }
}