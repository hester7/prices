using System.ComponentModel;

namespace Prices.Core.Domain.Enums;

public enum PriceIndexes
{
    [Description("CAISO - Day-Ahead")]
    CaisoDayAhead = 1,
    [Description("CAISO - Real-Time")]
    CaisoRealTime = 2,
    [Description("ERCOT - Day-Ahead")]
    ErcotDayAhead = 3,
    [Description("ERCOT - Real-Time")]
    ErcotRealTime = 4,

    //[Description("CAISO - Fifteen-Minute")]
    //CaisoFifteenMinute = 5,
    //[Description("ERCOT - Fifteen-Minute")]
    //ErcotFifteenMinute = 6,
}