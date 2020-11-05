using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// A service provider implementation which implements <see cref="ISupportRequiredService"/> and uses Stashbox to produce services.
    /// </summary>
    public class StashboxRequiredServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxRequiredServiceProvider"/>.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxRequiredServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType) => this.dependencyResolver.GetService(serviceType);

        /// <inheritdoc />
        public object GetRequiredService(Type serviceType) => this.dependencyResolver.Resolve(serviceType);

        /// <inheritdoc />
        public void Dispose() => this.dependencyResolver.Dispose();

        /// <inheritdoc />
        public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();
    }
}
