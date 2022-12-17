using Prices.Core.Domain.Enums;

namespace Prices.GraphQl.Types;

public readonly record struct RtoIdAndPricingNodeName(Rtos RtoId, string Name);