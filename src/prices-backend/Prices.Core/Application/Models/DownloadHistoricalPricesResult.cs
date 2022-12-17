namespace Prices.Core.Application.Models;

public readonly record struct DownloadHistoricalPricesResult(bool Success, IEnumerable<string> Errors);