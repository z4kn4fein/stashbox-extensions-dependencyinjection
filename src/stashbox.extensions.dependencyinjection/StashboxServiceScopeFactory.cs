using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.DependencyInjection;

/// <summary>
/// Represents a factory which produces service scoped with Stashbox.
/// </summary>
public sealed class StashboxServiceScopeFactory : IServiceScopeFactory
{
    private readonly IDependencyResolver dependencyResolver;

    /// <summary>
    /// Constructs a <see cref="StashboxServiceScopeFactory"/>.
    /// </summary>
    /// <param name="dependencyResolver">The Stashbox dependency resolver.</param>
    public StashboxServiceScopeFactory(IDependencyResolver dependencyResolver)
    {
        this.dependencyResolver = dependencyResolver;
    }

    /// <inheritdoc />
    public IServiceScope CreateScope() =>
        new StashboxServiceScope(this.dependencyResolver.BeginScope());
}