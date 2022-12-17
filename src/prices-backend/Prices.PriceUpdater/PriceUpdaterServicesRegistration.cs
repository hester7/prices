using Microsoft.Extensions.DependencyInjection;

namespace Prices.PriceUpdater
{
    public static class PriceUpdaterServicesRegistration
    {
        public static IServiceCollection AddPriceUpdaterServices(this IServiceCollection services) => services
            .AddHostedService<PriceUpdaterService>()
            .AddSingleton<ISeedPricingNodes, SeedPricingNodes>()
            .AddSingleton<ISeedPrices, SeedPrices>()
        ;
    }
}
