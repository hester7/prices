using NodaTime;
using Prices.Core.Domain.Enums;

namespace Prices.Core.Application.Models;

public class PriceByPricingNode
{
    public PriceIndexes PriceIndexId { get; set; }
    public Instant IntervalEndTimeUtc { get; set; }
    public decimal LmpPrice { get; set; }
}