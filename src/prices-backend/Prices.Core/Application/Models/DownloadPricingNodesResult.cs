using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models;

public readonly record struct DownloadPricingNodesResult(bool Success, IEnumerable<PricingNode> PricingNodes, IEnumerable<string> Errors);