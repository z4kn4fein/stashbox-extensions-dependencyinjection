using Microsoft.Extensions.DependencyInjection;
using Stashbox.Infrastructure;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents an <see cref="IServiceProvider"/> implementation based on the <see cref="IStashboxContainer"/>
    /// </summary>
    public class StashboxServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProvider"/>
        /// </summary>
        /// <param name="dependencyResolver">The resolution scope.</param>
        public StashboxServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType) =>
             this.dependencyResolver.Resolve(serviceType, nullResultAllowed: true);

        /// <inheritdoc />
        public object GetRequiredService(Type serviceType) =>
            this.dependencyResolver.Resolve(serviceType);

        /// <inheritdoc />
        public void Dispose() =>
            this.dependencyResolver.Dispose();
    }
}
