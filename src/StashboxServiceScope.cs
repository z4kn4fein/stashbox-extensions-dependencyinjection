using Microsoft.Extensions.DependencyInjection;
using Stashbox.Utils;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScope : IServiceScope
    {
        public StashboxServiceScope(IServiceProvider serviceProvider)
        {
            Shield.EnsureNotNull(serviceProvider, nameof(serviceProvider));

            this.ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose() => (ServiceProvider as IDisposable)?.Dispose();
    }
}
