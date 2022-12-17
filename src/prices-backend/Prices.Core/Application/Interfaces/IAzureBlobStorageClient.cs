using Prices.Core.Application.Models;

namespace Prices.Core.Application.Interfaces;

public interface IAzureBlobStorageClient
{
    Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName, CancellationToken cancellationToken = default);

    IEnumerable<BlobItem> GetBlobs(CancellationToken cancellationToken = default);

    Task<long> DownloadFileAsync(string blobName, string destinationPath, CancellationToken cancellationToken = default);

    Task UploadFileAsync<T>(string localFolder, string localFileName, string remoteFolder, string remoteFileName,
        T? metadata = default, CancellationToken cancellationToken = default);

    Task UploadFileAsync<T>(Stream stream, string remoteFolder, string remoteFileName, T? metadata = default, CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(string blobName, CancellationToken cancellationToken = default);
}