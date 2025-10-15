using Microsoft.Extensions.DependencyInjection;

namespace KwikNesta.Mediatrix.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKwikMediators(this IServiceCollection services)
        {
            // regular mediator
            services.AddSingleton<IKwikMediator, KwikMediator>();

            // background mediator
            //services.AddSingleton<IBackgroundMediator, BackgroundMediator>();

            // register all handlers dynamically
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                           (t.GetInterfaces().Any(i => i.IsGenericType &&
                               (i.GetGenericTypeDefinition() == typeof(IKwikRequestHandler<,>) ||
                                i.GetGenericTypeDefinition() == typeof(IKwikNotificationHandler<>))))
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
