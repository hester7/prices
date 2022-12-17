using Microsoft.Extensions.DependencyInjection;
using Prices.Core.Application.Interfaces.Factories;

namespace Prices.Core.Application.Extensions
{
    public static class AddServicesExtensions
    {
        // TODO: this works but isn't perfect
        public static IServiceCollection AddServices<TMarker, TServiceType>(this IServiceCollection services)
        {
            var assembly = typeof(TMarker).Assembly;
            var types = assembly.DefinedTypes.Where(x => typeof(TServiceType).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

            foreach (var type in types)
            {
                var allInterfaces = type.GetInterfaces();
                var minimalInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).ToList();
                if (minimalInterfaces.Count == 1)
                    services.AddSingleton(minimalInterfaces.First(), type);
            }

            return services;
        }

        // TODO: doesn't quite work
        public static IServiceCollection AddServiceByRtoFactory<TMarker>(this IServiceCollection services)
        {
            var assembly = typeof(TMarker).Assembly;
            var types = assembly.DefinedTypes.Where(x => IsAssignableToGenericType(x, typeof(IServiceByRtoFactory<>)) && !x.IsInterface && !x.IsAbstract);

            foreach (var type in types)
            {
                var allInterfaces = type.GetInterfaces();
                var minimalInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).ToList();
                if (minimalInterfaces.Count == 1)
                    services.AddSingleton(minimalInterfaces.First(), type);
            }

            return services;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}