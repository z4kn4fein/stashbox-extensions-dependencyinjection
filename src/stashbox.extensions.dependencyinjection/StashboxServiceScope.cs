using Microsoft.Extensions.DependencyInjection;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScope : IServiceScope
    {
        public StashboxServiceScope(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose() => (this.ServiceProvider as IDisposable)?.Dispose();
    }
}
