using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScope : IServiceScope, IAsyncDisposable
    {
        private readonly IDependencyResolver dependencyResolver;

        public StashboxServiceScope(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            this.ServiceProvider = new StashboxRequiredServiceProvider(dependencyResolver);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose() => this.dependencyResolver.Dispose();

        public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();
    }
}
