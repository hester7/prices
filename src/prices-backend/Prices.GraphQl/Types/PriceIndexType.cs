using Prices.Core.Application.Models;
using Prices.Core.Domain.Models;
using Prices.GraphQl.DataLoaders;

namespace Prices.GraphQl.Types;

[ExtendObjectType(typeof(PriceIndex),
    IgnoreProperties = new[]
    {
        nameof(PriceIndex.PriceMarket),
        nameof(PriceIndex.RegionalTransmissionOperator),
        nameof(PriceIndex.PricesFiles),
    })]
public sealed class PriceIndexType
{
    public async Task<IEnumerable<PriceByIndex>> GetPricesAsync(
        ChangeSpan span,
        [Parent] PriceIndex parent,
        PriceByPriceIndexIdDataLoader priceByPriceIndexId,
        CancellationToken cancellationToken)
    {
        return await priceByPriceIndexId.LoadAsync(new PriceIndexIdAndSpan(parent.Id, span), cancellationToken);
    }
}