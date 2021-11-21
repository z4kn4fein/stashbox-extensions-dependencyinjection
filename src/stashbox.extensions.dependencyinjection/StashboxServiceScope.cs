using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents a service scope which uses Stashbox.
    /// </summary>
    public class StashboxServiceScope : IServiceScope, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceScope"/>.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxServiceScope(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            this.ServiceProvider = new StashboxServiceProvider(dependencyResolver);
        }

        /// <inheritdoc />
        public IServiceProvider ServiceProvider { get; }

        /// <inheritdoc />
        public void Dispose() => this.dependencyResolver.Dispose();

        /// <inheritdoc />
        public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();
    }
}
