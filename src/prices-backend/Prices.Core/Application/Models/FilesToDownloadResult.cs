namespace Prices.Core.Application.Models;

public readonly record struct FilesToDownloadResult(bool Success, IEnumerable<PricesFileToDownload> FilesToDownload, IEnumerable<string> Errors);