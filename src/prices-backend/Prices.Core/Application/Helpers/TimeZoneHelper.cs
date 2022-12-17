using NodaTime;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Helpers
{
    public static class TimeZoneHelper
    {
        public static bool IsShortDay(DateTime date, string ianaTimeZoneId)
        {
            var transitions = GetDaylightSavingTransitions(ianaTimeZoneId, date.Year).ToList();
            if (!transitions.Any())
                return false;

            if (transitions.Count != 2)
                throw new NotSupportedException($"Time Zone {ianaTimeZoneId} is not supported.");

            return transitions.First().ToDateTimeUnspecified().Date == date.Date;
        }

        public static bool IsShortDay(DateTime date, Rtos rto) => IsShortDay(date, GetIanaTimeZoneId(rto));

        public static bool IsLongDay(DateTime date, string ianaTimeZoneId)
        {
            var transitions = GetDaylightSavingTransitions(ianaTimeZoneId, date.Year).ToList();
            if (!transitions.Any())
                return false;

            if (transitions.Count != 2)
                throw new NotSupportedException($"Time Zone {ianaTimeZoneId} is not supported.");

            return transitions.Last().ToDateTimeUnspecified().Date == date.Date;
        }

        public static bool IsLongDay(DateTime date, Rtos rto) => IsLongDay(date, GetIanaTimeZoneId(rto));

        public static IEnumerable<LocalDateTime> GetDaylightSavingTransitions(string timeZoneId, int year)
        {
            var timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];
            var yearStart = new LocalDateTime(year, 1, 1, 0, 0).InZoneLeniently(timeZone).ToInstant();
            var yearEnd = new LocalDateTime(year + 1, 1, 1, 0, 0).InZoneLeniently(timeZone).ToInstant();
            var intervals = timeZone.GetZoneIntervals(yearStart, yearEnd);
            return intervals.Select(x => x.IsoLocalEnd).Where(x => x.Year == year);
        }

        public static string GetIanaTimeZoneId(Rtos rto) =>
            rto switch
            {
                Rtos.CAISO => "America/Los_Angeles",
                Rtos.ERCOT => "America/Chicago",
                _ => throw new ArgumentOutOfRangeException(nameof(rto), rto, null)
            };

        public static DateTimeZone GetDateTimeZone(Rtos rto) => DateTimeZoneProviders.Tzdb[GetIanaTimeZoneId(rto)];

        public static string GetSqlTimeZoneId(Rtos rto) =>
            rto switch
            {
                Rtos.CAISO => "Pacific Standard Time",
                Rtos.ERCOT => "Central Standard Time",
                _ => throw new ArgumentOutOfRangeException(nameof(rto), rto, null)
            };
    }
}