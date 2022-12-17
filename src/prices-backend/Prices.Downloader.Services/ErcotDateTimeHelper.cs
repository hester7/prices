using NodaTime;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Helpers;
using Prices.Core.Domain.Enums;

namespace Prices.Downloader.Services
{
    public static class ErcotDateTimeHelper
    {
        private static readonly string TimeZoneId;

        static ErcotDateTimeHelper()
        {
            TimeZoneId = TimeZoneHelper.GetIanaTimeZoneId(Rtos.ERCOT);
        }

        public static void GetIntervalStartEndTimeUtc(string date, string hourEndingTime, string repeatedHourFlag, int intervalLength,
            out Instant intervalStartTimeUtc, out Instant intervalEndTimeUtc)
        {
            var localDate = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Unspecified);
            var utcDate = Instant.FromDateTimeUtc(localDate.InZone(TimeZoneId));
            var hour = int.Parse(hourEndingTime.Split(':')[0]);
            var repeatedHour = string.Equals(repeatedHourFlag, "Y", StringComparison.InvariantCultureIgnoreCase);
            var minutes = GetMinutes(localDate, hour, 0, repeatedHour);

            intervalStartTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes - intervalLength));
            intervalEndTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes));
        }

        public static void GetIntervalStartEndTimeUtc(string timestamp, string repeatedHourFlag, int intervalLength,
            out Instant intervalStartTimeUtc, out Instant intervalEndTimeUtc)
        {
            var localDate = DateTime.SpecifyKind(DateTime.Parse(timestamp).Date, DateTimeKind.Unspecified);
            var utcDate = Instant.FromDateTimeUtc(localDate.InZone(TimeZoneId));
            var hour = int.Parse(timestamp.Split(' ')[1].Split(':')[0]);
            var minute = int.Parse(timestamp.Split(' ')[1].Split(':')[1]);
            var repeatedHour = string.Equals(repeatedHourFlag, "Y", StringComparison.InvariantCultureIgnoreCase);
            var minutes = GetMinutes(localDate, hour, minute, repeatedHour);

            intervalStartTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes - intervalLength));
            intervalEndTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes));
        }

        private static int GetMinutes(DateTime date, int hour, int minute, bool repeatedHour)
        {
            if (TimeZoneHelper.IsShortDay(date, TimeZoneId) && hour > 2)
                hour--;

            if (TimeZoneHelper.IsLongDay(date, TimeZoneId) && (hour > 2 || hour == 2 && repeatedHour))
                hour++;

            return hour * 60 + minute;
        }

        public static void GetIntervalStartEndTimeUtc(string date, string hour, string interval, string repeatedHourFlag, int intervalLength,
            out Instant intervalStartTimeUtc, out Instant intervalEndTimeUtc)
        {
            var localDate = DateTime.SpecifyKind(DateTime.Parse(date), DateTimeKind.Unspecified);
            var utcDate = Instant.FromDateTimeUtc(localDate.InZone(TimeZoneId));
            var repeatedHour = string.Equals(repeatedHourFlag, "Y", StringComparison.InvariantCultureIgnoreCase);
            var minutes = GetMinutes(localDate, Convert.ToInt32(hour), repeatedHour, Convert.ToInt32(interval));

            intervalStartTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes - intervalLength));
            intervalEndTimeUtc = utcDate.Plus(Duration.FromMinutes(minutes));
        }

        private static int GetMinutes(DateTime date, int hour, bool repeatedHour, int interval)
        {
            if (!(TimeZoneHelper.IsLongDay(date, TimeZoneId) && (hour > 2 || hour == 2 && repeatedHour)))
                hour--;

            if (TimeZoneHelper.IsShortDay(date, TimeZoneId) && hour > 2)
                hour--;

            return hour * 60 + 15 * interval;
        }
    }
}