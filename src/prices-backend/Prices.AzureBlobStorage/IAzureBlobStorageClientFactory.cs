using Prices.Core.Application.Interfaces;

namespace Prices.AzureBlobStorage;

public interface IAzureBlobStorageClientFactory
{
    IAzureBlobStorageClient NewSasTokenClient(string sasUri);
}