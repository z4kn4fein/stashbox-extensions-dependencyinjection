using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// A service provider implementation which implements <see cref="ISupportRequiredService"/> and uses Stashbox to produce services.
    /// </summary>
    public sealed class StashboxServiceProvider : IServiceProvider, ISupportRequiredService,
#if HAS_IS_SERVICE
        IServiceProviderIsService, 
#endif
        IDisposable, IAsyncDisposable
    {
        private static readonly Type ServiceProviderType = typeof(IServiceProvider);

        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProvider"/>.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType) => 
            serviceType == ServiceProviderType ? this : this.dependencyResolver.GetService(serviceType);

        /// <inheritdoc />
        public object GetRequiredService(Type serviceType) =>
            serviceType == ServiceProviderType ? this : this.dependencyResolver.Resolve(serviceType);

        /// <inheritdoc />
        public void Dispose() => this.dependencyResolver.Dispose();

        /// <inheritdoc />
        public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();

#if HAS_IS_SERVICE
        /// <inheritdoc />
        public bool IsService(Type serviceType) => this.dependencyResolver.CanResolve(serviceType);
#endif
    }
}
