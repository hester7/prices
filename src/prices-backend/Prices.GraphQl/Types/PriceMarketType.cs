using Prices.Core.Domain.Models;

namespace Prices.GraphQl.Types;

[ExtendObjectType(typeof(PriceMarket), IgnoreProperties = new[] { nameof(PriceMarket.PriceIndexes) })]
public sealed class PriceMarketType
{
}
