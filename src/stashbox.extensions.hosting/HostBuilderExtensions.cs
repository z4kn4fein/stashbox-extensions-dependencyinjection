using Microsoft.Extensions.DependencyInjection;
using Stashbox;
using Stashbox.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Extensions for the <see cref="IHostBuilder"/> interface to configure an <see cref="IStashboxContainer"/> as the default <see cref="IServiceProvider"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
    /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseStashbox(this IHostBuilder builder, Action<IStashboxContainer> configure = null) =>
        builder.UseServiceProviderFactory(context => new StashboxServiceProviderFactory(container =>
        {
            if (context.HostingEnvironment.IsDevelopment())
                container.Configure(config => config.WithLifetimeValidation());

            configure?.Invoke(container);
        }));

    /// <summary>
    /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
    /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseStashbox(this IHostBuilder builder, IStashboxContainer container) =>
        builder.UseServiceProviderFactory(context =>
        {
            if (context.HostingEnvironment.IsDevelopment())
                container.Configure(config => config.WithLifetimeValidation());

            return new StashboxServiceProviderFactory(container);
        });

#if NET10_0_OR_GREATER
    /// <summary>
    /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
    /// <returns>The modified <see cref="IHostApplicationBuilder"/> instance.</returns>
    public static T UseStashbox<T>(this T builder, Action<IStashboxContainer> configure = null)
        where T : IHostApplicationBuilder
    {
        builder.ConfigureContainer(new StashboxServiceProviderFactory(configure), container =>
        {
            if (builder.Environment.IsDevelopment())
                container.Configure(config => config.WithLifetimeValidation());

            configure?.Invoke(container);
        });
        return builder;
    }

    /// <summary>
    /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> instance.</param>
    /// <param name="stashboxContainer">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
    /// <returns>The modified <see cref="IHostApplicationBuilder"/> instance.</returns>
    public static T UseStashbox<T>(this T builder, IStashboxContainer stashboxContainer)
        where T : IHostApplicationBuilder
    {
        builder.ConfigureContainer(new StashboxServiceProviderFactory(stashboxContainer), container =>
        {
            if (builder.Environment.IsDevelopment())
                container.Configure(config => config.WithLifetimeValidation());
        });
        return builder;
    }
#endif
}