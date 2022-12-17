using Microsoft.Extensions.DependencyInjection;

namespace Prices.AzureBlobStorage
{
    public static class AzureBlobStorageServicesRegistration
    {
        public static IServiceCollection AddAzureBlobStorageServices(this IServiceCollection services) => services
            .AddSingleton<IAzureBlobStorageClientFactory, AzureBlobStorageClientFactory>()
        ;
    }
}