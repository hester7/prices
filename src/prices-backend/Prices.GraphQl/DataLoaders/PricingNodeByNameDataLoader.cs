using Microsoft.EntityFrameworkCore;
using Prices.Core.Domain.Models;
using Prices.GraphQl.Types;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PricingNodeByNameDataLoader : BatchDataLoader<RtoIdAndPricingNodeName, PricingNode?>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PricingNodeByNameDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<RtoIdAndPricingNodeName, PricingNode?>> LoadBatchAsync(
        IReadOnlyList<RtoIdAndPricingNodeName> names,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var map = new Dictionary<RtoIdAndPricingNodeName, PricingNode?>();

        foreach (var rtoGroup in names.GroupBy(t => t.RtoId))
        {
            var pricingNodes = context.PricingNodes
                .Where(t => t.RegionalTransmissionOperatorId == rtoGroup.Key && rtoGroup.Select(g => g.Name).Contains(t.DisplayName ?? t.Name))
                .AsNoTracking()
                .ToList();

            foreach (var pn in pricingNodes)
            {
                map.Add(new RtoIdAndPricingNodeName(rtoGroup.Key, pn.DisplayName ?? pn.Name), pn);
            }
        }

        return map;
    }
}