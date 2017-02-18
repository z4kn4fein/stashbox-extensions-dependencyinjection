using Microsoft.Extensions.DependencyInjection;
using Stashbox.Infrastructure;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// Extensions of the <see cref="IWebHostBuilder"/> for adding <see cref="IStashboxContainer"/>.
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Sets the default service provider to <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> instance.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The modified <see cref="IWebHostBuilder"/> instance.</returns>
        public static IWebHostBuilder UseStashbox(this IWebHostBuilder builder, Action<IStashboxContainer> configure = null) =>
            builder.ConfigureServices(collection => collection.AddStashbox(configure));
    }
}
