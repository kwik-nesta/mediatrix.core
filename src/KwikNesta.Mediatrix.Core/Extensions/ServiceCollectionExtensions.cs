using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Core.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KwikNesta.Mediatrix.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers KwikMediator core services, handlers
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="assemblies">
        /// Optional assemblies to scan for handler implementations.
        /// If not provided, all currently loaded assemblies are scanned.
        /// </param>
        public static IServiceCollection AddKwikMediators(this IServiceCollection services,
                                                          params Assembly[] assemblies)
        {
            services.AddScoped<IKwikMediator, KwikMediator>();
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            assemblies = assemblies
                .Concat(new[] { typeof(IKwikMediator).Assembly })
                .Distinct()
                .ToArray();

            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                           t.GetInterfaces().Any(i => i.IsGenericType &&
                               (i.GetGenericTypeDefinition() == typeof(IKwikRequestHandler<,>) ||
                                i.GetGenericTypeDefinition() == typeof(IKwikNotificationHandler<>)))
                );

            foreach (var type in handlerTypes)
            {
                foreach (var i in type.GetInterfaces())
                {
                    if (i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IKwikRequestHandler<,>) ||
                         i.GetGenericTypeDefinition() == typeof(IKwikNotificationHandler<>)))
                    {
                        services.AddTransient(i, type);
                    }
                }
            }

            return services;
        }
    }
}
