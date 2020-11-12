using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stashbox.AspNetCore.Multitenant;
using Stashbox.Multitenant;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extensions for the <see cref="IHostBuilder"/> interface to configure Stashbox for multitenant applications.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Sets the Stashbox multitenant <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation as default.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="configure">The callback action to configure the internal <see cref="ITenantDistributor"/>.</param>
        /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder UseStashboxMultitenant<TTenantIdExtractor>(this IHostBuilder builder, Action<ITenantDistributor> configure = null)
            where TTenantIdExtractor : class, ITenantIdExtractor
        {
            var tenantDistributor = new TenantDistributor();
            configure?.Invoke(tenantDistributor);

            return builder.UseStashboxMultitenant<TTenantIdExtractor>(tenantDistributor);
        }

        /// <summary>
        /// Sets the Stashbox multitenant <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation as default.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="tenantDistributor">An already configured <see cref="ITenantDistributor"/> instance to use.</param>
        /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder UseStashboxMultitenant<TTenantIdExtractor>(this IHostBuilder builder, ITenantDistributor tenantDistributor)
            where TTenantIdExtractor : class, ITenantIdExtractor =>
            builder.UseServiceProviderFactory(new StashboxMultitenantServiceProviderFactory(tenantDistributor))
                .ConfigureContainer<ITenantDistributor>((context, dist) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                        dist.RootContainer.Configure(config => config.WithLifetimeValidation());
                })
                .ConfigureServices(services => 
                {
                    services.AddScoped<ITenantIdExtractor, TTenantIdExtractor>();
                    services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(p => new StashboxMultitenantStartupFilter()));
                });
    }
}
