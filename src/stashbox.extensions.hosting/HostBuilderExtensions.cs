using Stashbox;
using Stashbox.Extensions.Dependencyinjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// Extensions for the <see cref="IHostBuilder"/> to configure an <see cref="IStashboxContainer"/> as the default service provider.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Sets the default service provider to <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder UseStashbox(this IHostBuilder builder, System.Action<IStashboxContainer> configure = null) =>
            builder.UseServiceProviderFactory(context => new StashboxServiceProviderFactory(container =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                    container.Configure(config => config.WithLifetimeValidation());

                configure?.Invoke(container);
            }));

        /// <summary>
        /// Sets the default service provider to <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The modified <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder UseStashbox(this IHostBuilder builder, IStashboxContainer container) =>
            builder.UseServiceProviderFactory(new StashboxServiceProviderFactory(container));
    }
}
