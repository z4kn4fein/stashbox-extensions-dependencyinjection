using Microsoft.Extensions.DependencyInjection;
using Stashbox;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extensions for the <see cref="IWebHostBuilder"/> interface to configure an <see cref="IStashboxContainer"/> as the default <see cref="IServiceProvider"/>.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance.</param>
        /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The modified <see cref="IWebHostBuilder"/> instance.</returns>
        public static IWebHostBuilder UseStashbox(this IWebHostBuilder builder, Action<IStashboxContainer> configure = null) =>
            builder.ConfigureServices(collection => collection.AddStashbox(configure));

        /// <summary>
        /// Sets the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> to a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The modified <see cref="IWebHostBuilder"/> instance.</returns>
        public static IWebHostBuilder UseStashbox(this IWebHostBuilder builder, IStashboxContainer container) =>
            builder.ConfigureServices(collection => collection.AddStashbox(container));
    }
}
