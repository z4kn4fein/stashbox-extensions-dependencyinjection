using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScope : IServiceScope, IAsyncDisposable
    {
        public StashboxServiceScope(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose() => ((IDisposable)this.ServiceProvider).Dispose();

        public ValueTask DisposeAsync() => ((IAsyncDisposable)this.ServiceProvider).DisposeAsync();
    }
}
