using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using Prices.Core.Application.Interfaces;
using BlobItem = Prices.Core.Application.Models.BlobItem;

namespace Prices.AzureBlobStorage
{
    public class AzureBlobStorageClient : IAzureBlobStorageClient
    {
        private readonly BlobContainerClient _blobContainerClient;

        internal AzureBlobStorageClient(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<IDictionary<string, string>> GetBlobMetadataAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync(cancellationToken))
            {
                BlobProperties properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                return properties.Metadata;
            }

            throw new Exception($"Blob doesn't exist: {blobName}");
        }

        public IEnumerable<BlobItem> GetBlobs(CancellationToken cancellationToken = default)
        {
            var blobs = _blobContainerClient.GetBlobs(traits: BlobTraits.Metadata, cancellationToken: cancellationToken).AsPages();

            // Enumerate the blobs returned for each page.
            foreach (var blobPage in blobs)
            {
                foreach (var blobItem in blobPage.Values)
                {
                    yield return new BlobItem(blobItem.Name, blobItem.Metadata);
                }
            }
        }

        public async Task<long> DownloadFileAsync(string blobName, string destinationPath, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync(cancellationToken))
            {
                await blobClient.DownloadToAsync(destinationPath, cancellationToken);

                var props = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
                return props.Value.ContentLength;
            }

            throw new Exception($"Blob doesn't exist: {blobName}");
        }

        public async Task UploadFileAsync<T>(string localFolder, string localFileName, string remoteFolder, string remoteFileName,
            T? metadata = default, CancellationToken cancellationToken = default)
        {
            var localFilePath = Path.Combine(localFolder, localFileName);
            var blobName = !string.IsNullOrWhiteSpace(remoteFolder) ? Path.Combine(remoteFolder, remoteFileName) : remoteFileName;
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(localFilePath, true, cancellationToken).ConfigureAwait(false);
            await SetMetadata(metadata, blobClient, cancellationToken);
        }

        public async Task UploadFileAsync<T>(Stream stream, string remoteFolder, string remoteFileName,
            T? metadata = default, CancellationToken cancellationToken = default)
        {
            var blobName = !string.IsNullOrWhiteSpace(remoteFolder) ? Path.Combine(remoteFolder, remoteFileName) : remoteFileName;
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, true, cancellationToken).ConfigureAwait(false);
            await SetMetadata(metadata, blobClient, cancellationToken);
        }

        public async Task<bool> DeleteFileAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            return response.Value;
        }

        private static async Task SetMetadata<T>(T? metadata, BlobClient blobClient, CancellationToken cancellationToken)
        {
            if (metadata is null)
                return;

            // Set the blob's metadata.
            var json = JsonConvert.SerializeObject(metadata);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            await blobClient.SetMetadataAsync(dictionary, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}