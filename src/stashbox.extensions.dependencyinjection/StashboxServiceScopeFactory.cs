using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents a factory which produces service scoped with Stashbox.
    /// </summary>
    public sealed class StashboxServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceScopeFactory"/>.
        /// </summary>
        /// <param name="dependencyResolver">The stashbox dependency resolver.</param>
        public StashboxServiceScopeFactory(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public IServiceScope CreateScope() =>
            new StashboxServiceScope(this.dependencyResolver.BeginScope());
    }
}
