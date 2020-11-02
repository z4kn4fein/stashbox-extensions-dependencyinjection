using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxRequiredServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        public StashboxRequiredServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public object GetService(Type serviceType) => this.dependencyResolver.GetService(serviceType);

        public object GetRequiredService(Type serviceType) => this.dependencyResolver.Resolve(serviceType);

        public void Dispose() => this.dependencyResolver.Dispose();

        public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();
    }
}
