using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Stashbox;
using Stashbox.AspNetCore.Multitenant;
using Stashbox.Multitenant;
using System;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extensions for the <see cref="IHostBuilder"/> interface to configure Stashbox for multi-tenant applications.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Enables the Stashbox multi-tenant functionality. Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> with <see cref="StashboxMultitenantServiceProviderFactory"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="configure">The callback action to configure the multi-tenant behavior.</param>
    /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseStashboxMultitenant<TTenantIdExtractor>(this IHostBuilder builder, Action<StashboxMultitenantOptions>? configure = null)
        where TTenantIdExtractor : class, ITenantIdExtractor
    {
        var options = new StashboxMultitenantOptions(new StashboxContainer());
        configure?.Invoke(options);

        return builder.UseStashboxMultitenant<TTenantIdExtractor>(options);
    }

    /// <summary>
    /// Enables the Stashbox multi-tenant functionality. Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> with <see cref="StashboxMultitenantServiceProviderFactory"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="options">The multi-tenant options.</param>
    /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseStashboxMultitenant<TTenantIdExtractor>(this IHostBuilder builder, StashboxMultitenantOptions options)
        where TTenantIdExtractor : class, ITenantIdExtractor =>
        builder.UseServiceProviderFactory(new StashboxMultitenantServiceProviderFactory(options.RootContainer))
            .ConfigureContainer<IStashboxContainer>((context, dist) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                    dist.Configure(config => config.WithLifetimeValidation());
            })
            .ConfigureServices(services => 
            {
                services.AddScoped<ITenantIdExtractor, TTenantIdExtractor>();
                services.Insert(0, ServiceDescriptor.Transient<IStartupFilter>(_ => new StashboxMultitenantStartupFilter()));
            });
}