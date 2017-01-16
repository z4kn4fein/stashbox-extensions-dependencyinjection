using Microsoft.Extensions.DependencyInjection;
using Stashbox.Infrastructure;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal class StashboxServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IStashboxContainer stashboxContainer;

        public StashboxServiceScopeFactory(IStashboxContainer stashboxContainer)
        {
            this.stashboxContainer = stashboxContainer;
        }

        public IServiceScope CreateScope() => new StashboxServiceScope(new StashboxServiceProvider(this.stashboxContainer.BeginScope()));
    }
}
