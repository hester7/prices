using HotChocolate.Data.Filters;
using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using HotChocolate.Types.NodaTime;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Prices.Persistence.EntityFramework;

namespace Prices.GraphQl
{
    public static class GraphQlServicesRegistration
    {
        public static IRequestExecutorBuilder AddGraphQlServices(this IServiceCollection services) => services
            .AddGraphQLServer()
            .AddQueryType()
            .AddFiltering()
            .AddSorting()
            .AddProjections()
            .AddGraphQlTypes()
            .AddSubscriptionType()
            .AddInMemorySubscriptions()
            .RegisterService<ITopicEventSender>()
            .RegisterService<ITopicEventReceiver>()
            .RegisterDbContext<PricesContext>(DbContextKind.Pooled)
            .AddNodaTimeServices()
            .AddGlobalObjectIdentification()
        ;

        private static IRequestExecutorBuilder AddNodaTimeServices(this IRequestExecutorBuilder builder) => builder
            .AddType<InstantType>()
            .AddType<LocalDateType>()
            .AddConvention<IFilterConvention>(new FilterConventionExtension(x => x
                .BindRuntimeType<LocalDate, ComparableOperationFilterInputType<LocalDate>>()
                .BindRuntimeType<LocalDate?, ComparableOperationFilterInputType<LocalDate?>>()
                .BindRuntimeType<Instant, ComparableOperationFilterInputType<Instant>>()
                .BindRuntimeType<Instant?, ComparableOperationFilterInputType<Instant?>>()
            ))
        ;
    }
}
