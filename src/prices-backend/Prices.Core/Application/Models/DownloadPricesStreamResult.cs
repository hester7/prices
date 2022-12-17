namespace Prices.Core.Application.Models;

public readonly record struct DownloadPricesStreamResult(bool Success, Stream? Stream, PricesFileMetadata? Metadata, IEnumerable<string> Errors) : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        if (Stream != null) await Stream.DisposeAsync().ConfigureAwait(false);
    }
}