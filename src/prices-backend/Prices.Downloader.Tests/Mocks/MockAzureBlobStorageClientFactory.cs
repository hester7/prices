using Prices.AzureBlobStorage;
using Prices.Core.Application.Interfaces;

namespace Prices.Downloader.Tests.Mocks;

public class MockAzureBlobStorageClientFactory : IAzureBlobStorageClientFactory
{
    public IAzureBlobStorageClient NewSasTokenClient(string sasUri) => new MockAzureBlobStorageClient();
}