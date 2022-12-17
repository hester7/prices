using System.ComponentModel;

namespace Prices.Downloader.Services.Models;

public class ErcotHistoricalRtmRecord
{
    [Description("Delivery Date")]
    public string DeliveryDate { get; set; } = null!;
    [Description("Delivery Hour")]
    public string DeliveryHour { get; set; } = null!;
    [Description("Delivery Interval")]
    public string DeliveryInterval { get; set; } = null!;
    [Description("Repeated Hour Flag")]
    public string RepeatedHourFlag { get; set; } = null!;
    [Description("Settlement Point Name")]
    public string SettlementPointName { get; set; } = null!;
    [Description("Settlement Point Type")]
    public string SettlementPointType { get; set; } = null!;
    [Description("Settlement Point Price")]
    public string SettlementPointPrice { get; set; } = null!;
}