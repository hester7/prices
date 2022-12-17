using NodaTime;
using Prices.Core.Application.Extensions;
using Prices.Core.Application.Helpers;
using Prices.Core.Domain.Enums;
using Prices.Downloader.Services;

namespace Prices.Downloader.Tests;

public class NodaTimeTests
{
    private readonly DateTimeZone _ercotZone;

    public NodaTimeTests()
    {
        _ercotZone = TimeZoneHelper.GetDateTimeZone(Rtos.ERCOT);
    }

    [Fact]
    public void GetIntervalStartEndTimeUtc_DateHourEnding_Test()
    {
        var date = "07/01/2021";
        var hourEndingTime = "01:00";
        var repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out var intervalStartTimeUtc, out var intervalEndTimeUtc);
        var expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 05:00"), DateTimeKind.Utc));
        var expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 06:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        var zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        var zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "12/01/2021";
        hourEndingTime = "01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 06:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hourEndingTime = "01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 06:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hourEndingTime = "02:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hourEndingTime = "04:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 09:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hourEndingTime = "01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 05:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 06:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hourEndingTime = "02:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 06:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hourEndingTime = "02:00";
        repeatedHourFlag = "Y";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hourEndingTime = "03:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hourEndingTime, repeatedHourFlag, 60, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 09:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);
    }

    [Fact]
    public void GetIntervalStartEndTimeUtc_Timestamp_Test()
    {
        var timestamp = "07/01/2021 01:00";
        var repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out var intervalStartTimeUtc, out var intervalEndTimeUtc);
        var expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 05:55"), DateTimeKind.Utc));
        var expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 06:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        var zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        var zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "12/01/2021 01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 06:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "03/14/2021 01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 06:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "03/14/2021 02:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "03/14/2021 04:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 09:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "11/07/2021 01:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 05:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 06:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "11/07/2021 02:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 06:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "11/07/2021 02:00";
        repeatedHourFlag = "Y";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        timestamp = "11/07/2021 03:00";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(timestamp, repeatedHourFlag, 5, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:55"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 09:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);
    }

    [Fact]
    public void GetIntervalStartEndTimeUtc_DateHourInterval_Test()
    {
        var date = "07/01/2021";
        var hour = "1";
        var interval = "4";
        var repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out var intervalStartTimeUtc, out var intervalEndTimeUtc);
        var expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 05:45"), DateTimeKind.Utc));
        var expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-07-01 06:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        var zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        var zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "12/01/2021";
        hour = "1";
        interval = "4";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 06:45"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-12-01 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hour = "2";
        interval = "3";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:30"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:45"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hour = "2";
        interval = "4";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 07:45"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "03/14/2021";
        hour = "4";
        interval = "1";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-03-14 08:15"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hour = "2";
        interval = "4";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 06:45"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.NotEqual(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hour = "2";
        interval = "1";
        repeatedHourFlag = "Y";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:15"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hour = "2";
        interval = "4";
        repeatedHourFlag = "Y";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 07:45"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:00"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);

        date = "11/07/2021";
        hour = "3";
        interval = "1";
        repeatedHourFlag = "N";

        ErcotDateTimeHelper.GetIntervalStartEndTimeUtc(date, hour, interval, repeatedHourFlag, 15, out intervalStartTimeUtc, out intervalEndTimeUtc);
        expectedIntervalStartTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:00"), DateTimeKind.Utc));
        expectedIntervalEndTimeUtc = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2021-11-07 08:15"), DateTimeKind.Utc));
        Assert.Equal(expectedIntervalStartTimeUtc, intervalStartTimeUtc);
        Assert.Equal(expectedIntervalEndTimeUtc, intervalEndTimeUtc);

        zonedIntervalStartTimeUtc = intervalStartTimeUtc.InZone(_ercotZone);
        zonedIntervalEndTimeUtc = intervalEndTimeUtc.InZone(_ercotZone);
        Assert.Equal(zonedIntervalStartTimeUtc.Offset, zonedIntervalEndTimeUtc.Offset);
    }

    [Fact]
    public void IsShortDayTest()
    {
        Assert.False(TimeZoneHelper.IsShortDay(new DateTime(2022, 1, 1), Rtos.CAISO));
        Assert.True(TimeZoneHelper.IsShortDay(new DateTime(2022, 3, 13), Rtos.CAISO));
        Assert.False(TimeZoneHelper.IsShortDay(new DateTime(2022, 11, 6), Rtos.CAISO));
    }

    [Fact]
    public void IsLongDayTest()
    {
        Assert.False(TimeZoneHelper.IsLongDay(new DateTime(2022, 1, 1), Rtos.CAISO));
        Assert.False(TimeZoneHelper.IsLongDay(new DateTime(2022, 3, 13), Rtos.CAISO));
        Assert.True(TimeZoneHelper.IsLongDay(new DateTime(2022, 11, 6), Rtos.CAISO));
    }

    [Fact]
    public void DateRoundUpTest()
    {
        var utcDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2022-01-01 16:59"), DateTimeKind.Utc));
        var roundupDate = utcDate.RoundUp(TimeSpan.FromMinutes(15));
        Assert.Equal(17, roundupDate.ToDateTimeUtc().Hour);
        Assert.Equal(0, roundupDate.ToDateTimeUtc().Minute);

        utcDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2022-01-01 17:00"), DateTimeKind.Utc));
        roundupDate = utcDate.RoundUp(TimeSpan.FromMinutes(15));
        Assert.Equal(17, roundupDate.ToDateTimeUtc().Hour);
        Assert.Equal(0, roundupDate.ToDateTimeUtc().Minute);

        utcDate = Instant.FromDateTimeUtc(DateTime.SpecifyKind(DateTime.Parse("2022-01-01 17:01"), DateTimeKind.Utc));
        roundupDate = utcDate.RoundUp(TimeSpan.FromMinutes(15));
        Assert.Equal(17, roundupDate.ToDateTimeUtc().Hour);
        Assert.Equal(15, roundupDate.ToDateTimeUtc().Minute);
    }
}