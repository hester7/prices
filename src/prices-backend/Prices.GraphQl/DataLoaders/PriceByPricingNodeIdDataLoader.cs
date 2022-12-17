using Microsoft.EntityFrameworkCore;
using Prices.Core.Application.Models;
using Prices.GraphQl.Helpers;
using Prices.GraphQl.Types;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PriceByPricingNodeIdDataLoader : BatchDataLoader<PricingNodeIdAndSpan, IEnumerable<PriceByPricingNode>>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PriceByPricingNodeIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<PricingNodeIdAndSpan, IEnumerable<PriceByPricingNode>>> LoadBatchAsync(
        IReadOnlyList<PricingNodeIdAndSpan> filters,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var map = new Dictionary<PricingNodeIdAndSpan, IEnumerable<PriceByPricingNode>>();

        foreach (var spanGroup in filters.GroupBy(t => t.Span))
        {
            var query = PricesQueryHelper.GetPricesForSpanQuery(spanGroup.Key, pricingNodes: spanGroup.Select(i => i.PricingNodeId));

            var prices = context.Prices
                .FromSqlRaw(query)
                .AsNoTracking()
                .Select(p => new { p.PriceIndexId, p.PricingNodeId, p.LmpPrice, p.IntervalEndTimeUtc })
                .AsEnumerable()
                .ToList();

            if (prices.Any())
            {
                foreach (var pricesGroup in prices.GroupBy(p => p.PricingNodeId))
                {
                    var priceMap = pricesGroup
                        .Select(p =>
                            new PriceByPricingNode
                            {
                                PriceIndexId = p.PriceIndexId,
                                IntervalEndTimeUtc = p.IntervalEndTimeUtc,
                                LmpPrice = p.LmpPrice
                            });

                    map.Add(new PricingNodeIdAndSpan(pricesGroup.Key, spanGroup.Key), priceMap);
                }
            }
            else
            {
                foreach (var pricingNodeId in spanGroup.Select(i => i.PricingNodeId))
                {
                    map.Add(new PricingNodeIdAndSpan(pricingNodeId, spanGroup.Key), Enumerable.Empty<PriceByPricingNode>());
                }
            }
        }

        return map;
    }
}