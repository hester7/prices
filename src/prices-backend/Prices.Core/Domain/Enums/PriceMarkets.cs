using System.ComponentModel;

namespace Prices.Core.Domain.Enums
{
    public enum PriceMarkets
    {
        [Description("Day-Ahead")]
        DAM = 1,
        [Description("Real-Time")]
        RTM = 2,
        //[Description("Fifteen-Minute")]
        //FMM = 3,
    }
}
