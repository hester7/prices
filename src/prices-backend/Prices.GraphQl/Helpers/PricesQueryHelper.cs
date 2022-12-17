using Prices.GraphQl.Types;

namespace Prices.GraphQl.Helpers
{
    public static class PricesQueryHelper
    {
        internal static string GetPricesForSpanQuery(ChangeSpan span, IEnumerable<int>? pricingNodes = null, IEnumerable<int>? priceIndexes = null)
        {
            int? minuteMod = null;
            int? hourMod = null;
            int? dayMod = null;
            int? hour = null;
            DateTime? minIntervalEndTimeUtc = null;

            switch (span)
            {
                case ChangeSpan.All:
                    minuteMod = 60;
                    dayMod = 7;
                    hour = DateTime.UtcNow.AddHours(-1).Hour;
                    break;
                case ChangeSpan.Hour:
                    minIntervalEndTimeUtc = DateTime.UtcNow.AddHours(-1);
                    break;
                case ChangeSpan.Day:
                    minuteMod = 10;
                    minIntervalEndTimeUtc = DateTime.UtcNow.AddDays(-1);
                    break;
                case ChangeSpan.Week:
                    minuteMod = 30;
                    minIntervalEndTimeUtc = DateTime.UtcNow.AddDays(-7);
                    break;
                case ChangeSpan.Month:
                    minuteMod = 60;
                    hourMod = 2;
                    minIntervalEndTimeUtc = DateTime.UtcNow.AddMonths(-1);
                    break;
                case ChangeSpan.Year:
                    minuteMod = 60;
                    hour = DateTime.UtcNow.AddHours(-1).Hour;
                    minIntervalEndTimeUtc = DateTime.UtcNow.AddYears(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var where = new List<string>();
            if (minuteMod.HasValue)
                where.Add($@"mod(cast(case when date_part('minute', p.""IntervalEndTimeUtc"") = 0 then 60 else date_part('minute', p.""IntervalEndTimeUtc"") end as int), {minuteMod}) = 0");
            if (hourMod.HasValue)
                where.Add($@"mod(cast(date_part('hour', p.""IntervalEndTimeUtc"") as int), {hourMod}) = 0");
            if (dayMod.HasValue)
                where.Add($@"mod(cast(date_part('day', p.""IntervalEndTimeUtc"") as int), {dayMod}) = 0");
            if (hour.HasValue)
                where.Add($@"date_part('hour', p.""IntervalEndTimeUtc"") = {hour}");
            if (minIntervalEndTimeUtc.HasValue)
                where.Add($@"p.""IntervalEndTimeUtc"" >= '{minIntervalEndTimeUtc:u}'");
            if (span == ChangeSpan.Hour)
                where.Add($@"p.""IntervalEndTimeUtc"" <= '{DateTime.UtcNow:u}'");
            if (pricingNodes is not null)
                where.Add($@"p.""PricingNodeId"" in ({string.Join(",", pricingNodes)})");
            if (priceIndexes is not null)
                where.Add($@"p.""PriceIndexId"" in ({string.Join(",", priceIndexes)})");

            return $@"
                select p.""PriceIndexId"", p.""PricingNodeId"", p.""PricingNodeName"", p.""LmpPrice"", p.""IntervalEndTimeUtc""
                from ""Prices"" p
                where {string.Join(" and ", where)}";
        }
    }
}