using Microsoft.EntityFrameworkCore;
using Prices.Core.Application.Models;
using Prices.GraphQl.Helpers;
using Prices.GraphQl.Types;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.DataLoaders;

public sealed class PriceByPriceIndexIdDataLoader : BatchDataLoader<PriceIndexIdAndSpan, IEnumerable<PriceByIndex>>
{
    private readonly IDbContextFactory<PricesContext> _contextFactory;

    public PriceByPriceIndexIdDataLoader(
        IDbContextFactory<PricesContext> contextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    protected override async Task<IReadOnlyDictionary<PriceIndexIdAndSpan, IEnumerable<PriceByIndex>>> LoadBatchAsync(
        IReadOnlyList<PriceIndexIdAndSpan> filters,
        CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var map = new Dictionary<PriceIndexIdAndSpan, IEnumerable<PriceByIndex>>();

        foreach (var spanGroup in filters.GroupBy(t => t.Span))
        {
            var query = PricesQueryHelper.GetPricesForSpanQuery(spanGroup.Key, priceIndexes: spanGroup.Select(i => (int)i.PriceIndexId));

            var prices = context.Prices
                .FromSqlRaw(query)
                .AsNoTracking()
                .Select(p => new { p.PriceIndexId, p.PricingNodeName, p.LmpPrice, p.IntervalEndTimeUtc })
                .AsEnumerable()
                .ToList();

            if (prices.Any())
            {
                foreach (var pricesGroup in prices.GroupBy(p => p.PriceIndexId))
                {
                    var priceMap = pricesGroup
                        .Select(p =>
                            new PriceByIndex
                            {
                                PriceIndexId = p.PriceIndexId,
                                PricingNodeName = p.PricingNodeName,
                                IntervalEndTimeUtc = p.IntervalEndTimeUtc,
                                LmpPrice = p.LmpPrice
                            });

                    map.Add(new PriceIndexIdAndSpan(pricesGroup.Key, spanGroup.Key), priceMap);
                }
            }
            else
            {
                foreach (var priceIndexId in spanGroup.Select(i => i.PriceIndexId))
                {
                    map.Add(new PriceIndexIdAndSpan(priceIndexId, spanGroup.Key), Enumerable.Empty<PriceByIndex>());
                }
            }
        }

        return map;
    }

    
}