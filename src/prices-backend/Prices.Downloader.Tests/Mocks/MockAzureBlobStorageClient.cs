using Prices.Core.Application.Extensions;
using Prices.Core.Application.Interfaces;
using Prices.Core.Application.Models;

namespace Prices.Downloader.Tests.Mocks;

public class MockAzureBlobStorageClient : IAzureBlobStorageClient
{
    public Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName, CancellationToken cancellationToken = default) =>
        ((IDictionary<string, string>)new Dictionary<string, string>()).AsTask();

    public IEnumerable<BlobItem> GetBlobs(CancellationToken cancellationToken = default) => Enumerable.Empty<BlobItem>();

    public Task<long> DownloadFileAsync(string blobName, string destinationPath, CancellationToken cancellationToken = default) => Convert.ToInt64(0).AsTask();

    public Task UploadFileAsync<T>(string localFolder, string localFileName, string remoteFolder, string remoteFileName,
        T? metadata = default, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task UploadFileAsync<T>(Stream stream, string remoteFolder, string remoteFileName, T? metadata = default,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<bool> DeleteFileAsync(string blobName, CancellationToken cancellationToken = default) => true.AsTask();
}