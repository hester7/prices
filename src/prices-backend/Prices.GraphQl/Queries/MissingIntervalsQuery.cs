using Microsoft.EntityFrameworkCore;
using NodaTime;
using Prices.Core.Application.Helpers;
using Prices.Core.Domain.Enums;
using Prices.Core.Domain.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class MissingIntervalsQuery
{
    [UsePaging]
    [UseSorting]
    public async Task<IEnumerable<IntervalEndTimeUtcScalar>> GetMissingIntervals(PricesContext context, PriceIndexes priceIndex, LocalDate startDate, LocalDate endDate)
    {
        var index = await context.PriceIndexes.FindAsync(priceIndex);

        var price = await context.Prices.FirstOrDefaultAsync(p => p.PriceIndexId == priceIndex);
        if (price is null)
            throw new Exception($"No prices found for price index {priceIndex}");

        var timeZone = TimeZoneHelper.GetIanaTimeZoneId(index!.RegionalTransmissionOperatorId);
        var intervalLength = $"{price.IntervalLength} minute";

        // TODO: EF Core 7 (Querying scalar (non-entity) types)
        // https://learn.microsoft.com/en-us/ef/core/querying/sql-queries#querying-scalar-non-entity-types

        FormattableString query = $@"
            with intervals as (
	            select generate_series({startDate}::timestamp at time zone {timeZone}, {endDate}::timestamp at time zone {timeZone}, {intervalLength}::interval)
            ),
            utcIntervals as (
	            select generate_series at time zone 'UTC' as intervalEndTimeUtc
	            from intervals as i
            )
            select i.intervalEndTimeUtc::timestamptz
            from utcIntervals as i
            left outer join ""Prices"" as p on i.intervalEndTimeUtc = p.""IntervalEndTimeUtc"" AND p.""PriceIndexId"" = {(int)index.Id}
            where p.""IntervalEndTimeUtc"" is null";

        var missingIntervals = context
            .Set<IntervalEndTimeUtcScalar>()
            .FromSqlInterpolated(query)
            .AsEnumerable()
            .ToList();

        return missingIntervals;
    }
}