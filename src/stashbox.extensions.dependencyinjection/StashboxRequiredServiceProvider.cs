using Microsoft.Extensions.DependencyInjection;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxRequiredServiceProvider : IServiceProvider, ISupportRequiredService
    {
        private readonly IDependencyResolver dependencyResolver;

        public StashboxRequiredServiceProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public object GetService(Type serviceType) => this.dependencyResolver.GetService(serviceType);

        public object GetRequiredService(Type serviceType) => this.dependencyResolver.Resolve(serviceType);
    }
}
