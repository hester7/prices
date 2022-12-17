using System.ComponentModel;

namespace Prices.Downloader.Services.Models;

public class ErcotHistoricalDamRecord
{
    [Description("Delivery Date")]
    public string DeliveryDate { get; set; } = null!;
    [Description("Hour Ending")]
    public string HourEnding { get; set; } = null!;
    [Description("Repeated Hour Flag")]
    public string RepeatedHourFlag { get; set; } = null!;
    [Description("Settlement Point")]
    public string SettlementPoint { get; set; } = null!;
    [Description("Settlement Point Price")]
    public string SettlementPointPrice { get; set; } = null!;
}