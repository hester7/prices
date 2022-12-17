namespace Prices.Core.Application.Models;

public readonly record struct DownloadPricesFileResult(bool Success, PricesFileMetadata? Metadata, IEnumerable<string> Errors);