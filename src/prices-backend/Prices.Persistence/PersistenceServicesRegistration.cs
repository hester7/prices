using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Prices.Core.Application.Models;
using Prices.Persistence.EntityFramework;

namespace Prices.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, Settings settings) => services
            .AddEntityFrameworkServices(settings.SqlConnection);

        public static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, string connectionString) => services
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddPooledDbContextFactory<PricesContext>(o => o.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure();
                options.CommandTimeout(300);
                options.UseNodaTime();
            }))
        ;
    }
}
