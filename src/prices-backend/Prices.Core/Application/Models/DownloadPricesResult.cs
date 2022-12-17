using Prices.Core.Domain.Models;

namespace Prices.Core.Application.Models;

public readonly record struct DownloadPricesResult(
    bool Success,
    PricesFileMetadata? Metadata,
    IEnumerable<Price>? Prices,
    IEnumerable<string> Errors,
    IEnumerable<string> Warnings);
