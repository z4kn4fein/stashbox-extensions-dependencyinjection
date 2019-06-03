using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IDependencyResolver dependencyResolver;

        public StashboxServiceScopeFactory(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public IServiceScope CreateScope()
        {
#if HAS_SERVICEPROVIDER
            return new StashboxServiceScope(this.dependencyResolver.BeginScope());
#else
            return new StashboxServiceScope(new StashboxServiceProvider(this.dependencyResolver.BeginScope()));
#endif
        }
    }
}
