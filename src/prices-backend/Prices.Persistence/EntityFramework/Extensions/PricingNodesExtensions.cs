using Microsoft.EntityFrameworkCore;
using NodaTime;
using Prices.Core.Application.Models;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.Extensions
{
    public static class PricingNodesExtensions
    {
        public static async Task UpsertRangeAsync(this DbSet<PricingNode> pricingNodes, IEnumerable<PricingNode> pricingNodesToUpsert,
            CancellationToken cancellationToken = default)
        {
            var pricingNodesToUpsertDict = pricingNodesToUpsert.ToDictionary(pn => pn.Name, pn => pn);
            var existingPricingNodesDict = await pricingNodes.ToDictionaryAsync(pn => pn.Name, pn => pn, cancellationToken: cancellationToken);

            foreach (var pricingNodeToUpsertDict in pricingNodesToUpsertDict)
            {
                existingPricingNodesDict.TryGetValue(pricingNodeToUpsertDict.Key, out var existingPricingNode);

                if (existingPricingNode is null)
                {
                    await pricingNodes.AddAsync(pricingNodeToUpsertDict.Value, cancellationToken);
                    continue;
                }

                existingPricingNode.PricingNodeTypeId = pricingNodeToUpsertDict.Value.PricingNodeTypeId;
                existingPricingNode.StartDateUtc = pricingNodeToUpsertDict.Value.StartDateUtc;
                existingPricingNode.EndDateUtc = pricingNodeToUpsertDict.Value.EndDateUtc;
                existingPricingNode.LastModifiedAtUtc = pricingNodeToUpsertDict.Value.LastModifiedAtUtc;
            }
        }

        public static async Task UpdateCurrentPricesAsync(this DbSet<PricingNode> pricingNodes, IEnumerable<Price> currentPrices,
            IEnumerable<Price> prices24HoursAgo, IClock clock, CancellationToken cancellationToken = default)
        {
            var currentPricesGrouped = currentPrices
                .GroupBy(p => new { p.PricingNodeId })
                .Select(p => new PricingNode
                {
                    Id = p.Key.PricingNodeId,
                    CurrentPrice = p.Single().LmpPrice
                });

            var pricingNodesToUpdate =
                from currentPrice in currentPricesGrouped
                join price24HoursAgo in prices24HoursAgo on currentPrice.Id equals price24HoursAgo.PricingNodeId into pn
                from p24 in pn.DefaultIfEmpty()
                select new PricingNodeWithPrice24HoursAgo
                {
                    Id = currentPrice.Id,
                    CurrentPrice = currentPrice.CurrentPrice,
                    Price24HoursAgo = p24?.LmpPrice ?? null
                };

            var pricingNodesToUpdateDict = pricingNodesToUpdate.ToDictionary(pn => pn.Id, pn => pn);
            var existingPricingNodesDict = await pricingNodes.ToDictionaryAsync(pn => pn.Id, pn => pn, cancellationToken: cancellationToken);

            foreach (var pricingNodeToUpdateDict in pricingNodesToUpdateDict)
            {
                existingPricingNodesDict.TryGetValue(pricingNodeToUpdateDict.Key, out var existingPricingNode);

                if (existingPricingNode is not null)
                {
                    existingPricingNode.LastModifiedAtUtc = clock.GetCurrentInstant();
                    existingPricingNode.CurrentPrice = pricingNodeToUpdateDict.Value.CurrentPrice;
                    existingPricingNode.Change24Hour = pricingNodeToUpdateDict.Value.Price24HoursAgo.HasValue
                        ? pricingNodeToUpdateDict.Value.CurrentPrice - pricingNodeToUpdateDict.Value.Price24HoursAgo.Value
                        : null;
                }
            }
        }
    }
}