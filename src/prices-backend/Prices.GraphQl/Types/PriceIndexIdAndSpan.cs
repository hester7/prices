using Prices.Core.Domain.Enums;

namespace Prices.GraphQl.Types;

public readonly record struct PriceIndexIdAndSpan(PriceIndexes PriceIndexId, ChangeSpan Span);