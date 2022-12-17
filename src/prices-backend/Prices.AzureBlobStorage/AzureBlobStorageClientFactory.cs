using Azure.Storage.Blobs;
using Prices.Core.Application.Interfaces;

namespace Prices.AzureBlobStorage;

public class AzureBlobStorageClientFactory : IAzureBlobStorageClientFactory
{
    public IAzureBlobStorageClient NewSasTokenClient(string sasUri)
    {
        var uriBuilder = new UriBuilder(sasUri);
        var blobContainerClient = new BlobContainerClient(uriBuilder.Uri);
        return new AzureBlobStorageClient(blobContainerClient);
    }
}