using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Prices.Core.Domain.Models;

namespace Prices.Persistence.EntityFramework.Extensions
{
    public static class PriceExtensions
    {
        public static async Task BulkMergeAsync(this DbSet<Price> dbSet,
            List<Price> prices,
            CancellationToken cancellationToken = default)
        {
            var dbContext = dbSet.GetService<ICurrentDbContext>().Context;

            var pricesTableName = $"{nameof(Price)}s";
            var tempTableName = $"temp_prices_{Guid.NewGuid():N}";

            await dbContext.Database.ExecuteSqlRawAsync($@"CREATE TABLE {tempTableName} (LIKE ""{pricesTableName}"" INCLUDING ALL)", cancellationToken);

            var bulkConfig = new BulkConfig
            {
                CustomDestinationTableName = tempTableName,
            };
            await dbContext.BulkInsertAsync(prices, bulkConfig, cancellationToken: cancellationToken);

            var mergeSql = $@"
                    MERGE INTO ""{pricesTableName}"" t
                    USING {tempTableName} s
                    ON t.""{nameof(Price.PriceIndexId)}"" = s.""{nameof(Price.PriceIndexId)}""
                        AND t.""{nameof(Price.PricingNodeId)}"" = s.""{nameof(Price.PricingNodeId)}""
                        AND t.""{nameof(Price.IntervalEndTimeUtc)}"" = s.""{nameof(Price.IntervalEndTimeUtc)}""
                    WHEN MATCHED THEN
                        UPDATE SET
                            ""{nameof(Price.IntervalStartTimeUtc)}"" = s.""{nameof(Price.IntervalStartTimeUtc)}"",
                            ""{nameof(Price.IntervalLength)}"" = s.""{nameof(Price.IntervalLength)}"",
                            ""{nameof(Price.LmpPrice)}"" = s.""{nameof(Price.LmpPrice)}"",
                            ""{nameof(Price.EnergyPrice)}"" = s.""{nameof(Price.EnergyPrice)}"",
                            ""{nameof(Price.CongestionPrice)}"" = s.""{nameof(Price.CongestionPrice)}"",
                            ""{nameof(Price.LossPrice)}"" = s.""{nameof(Price.LossPrice)}"",
                            ""{nameof(Price.PricingNodeName)}"" = s.""{nameof(Price.PricingNodeName)}"",
                            ""{nameof(Price.LastModifiedAtUtc)}"" = s.""{nameof(Price.LastModifiedAtUtc)}""
                    WHEN NOT MATCHED THEN
                        INSERT (""{nameof(Price.PriceIndexId)}"",
                                ""{nameof(Price.PricingNodeId)}"",
                                ""{nameof(Price.IntervalStartTimeUtc)}"",
                                ""{nameof(Price.IntervalEndTimeUtc)}"",
                                ""{nameof(Price.IntervalLength)}"",
                                ""{nameof(Price.LmpPrice)}"",
                                ""{nameof(Price.EnergyPrice)}"",
                                ""{nameof(Price.CongestionPrice)}"",
                                ""{nameof(Price.LossPrice)}"",
                                ""{nameof(Price.PricingNodeName)}"",
                                ""{nameof(Price.LastModifiedAtUtc)}"")
                        VALUES (s.""{nameof(Price.PriceIndexId)}"",
                                s.""{nameof(Price.PricingNodeId)}"",
                                s.""{nameof(Price.IntervalStartTimeUtc)}"",
                                s.""{nameof(Price.IntervalEndTimeUtc)}"",
                                s.""{nameof(Price.IntervalLength)}"",
                                s.""{nameof(Price.LmpPrice)}"",
                                s.""{nameof(Price.EnergyPrice)}"",
                                s.""{nameof(Price.CongestionPrice)}"",
                                s.""{nameof(Price.LossPrice)}"",
                                s.""{nameof(Price.PricingNodeName)}"",
                                s.""{nameof(Price.LastModifiedAtUtc)}"")";

            await dbContext.Database.ExecuteSqlRawAsync(mergeSql, cancellationToken);

            await dbContext.Database.ExecuteSqlRawAsync($"DROP TABLE \"{tempTableName}\"", cancellationToken);
        }
    }
}
