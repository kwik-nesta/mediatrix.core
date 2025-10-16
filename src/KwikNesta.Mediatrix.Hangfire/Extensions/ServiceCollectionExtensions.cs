using KwikNesta.Mediatrix.Hangfire.Abstractions;
using KwikNesta.Mediatrix.Hangfire.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace KwikNesta.Mediatrix.Hangfire.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Hangfire backgound job mediator registrar
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKwikBackgroundMediators(this IServiceCollection services)
        {
            services.AddSingleton<IKwikBackgroundMediator, KwikBackgroundMediator>();
            return services;
        }
    }
}
