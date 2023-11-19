using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Stashbox.Extensions.DependencyInjection;

/// <summary>
/// An <see cref="IServiceProvider"/> implementation that uses Stashbox to produce services.
/// </summary>
public sealed class StashboxServiceProvider : IServiceProvider, ISupportRequiredService,
#if HAS_IS_SERVICE
    IServiceProviderIsService,
#endif
#if HAS_KEYED
    IKeyedServiceProvider,
    IServiceProviderIsKeyedService,
#endif
    IDisposable, IAsyncDisposable
{
    private static readonly Type ServiceProviderType = typeof(IServiceProvider);

    private readonly IDependencyResolver dependencyResolver;

    internal IDependencyResolver DependencyResolver => dependencyResolver;

    /// <summary>
    /// Constructs a <see cref="StashboxServiceProvider"/>.
    /// </summary>
    /// <param name="dependencyResolver">A Stashbox <see cref="IDependencyResolver"/> implementation.</param>
    public StashboxServiceProvider(IDependencyResolver dependencyResolver)
    {
        this.dependencyResolver = dependencyResolver;
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType) =>
        serviceType == ServiceProviderType ? this : this.dependencyResolver.ResolveOrDefault(serviceType);

    /// <inheritdoc />
    public object GetRequiredService(Type serviceType) =>
        serviceType == ServiceProviderType ? this : this.dependencyResolver.Resolve(serviceType);

    /// <inheritdoc />
    public void Dispose() => this.dependencyResolver.Dispose();

    /// <inheritdoc />
    public ValueTask DisposeAsync() => this.dependencyResolver.DisposeAsync();

#if HAS_IS_SERVICE
    /// <inheritdoc />
    public bool IsService(Type serviceType) => this.dependencyResolver.CanResolve(serviceType);
#endif

#if HAS_KEYED
    /// <inheritdoc />
    public object? GetKeyedService(Type serviceType, object? serviceKey) =>
        this.dependencyResolver.ResolveOrDefault(serviceType, serviceKey);

    /// <inheritdoc />
    public object GetRequiredKeyedService(Type serviceType, object? serviceKey) =>
        this.dependencyResolver.Resolve(serviceType, serviceKey);
    
    /// <inheritdoc />
    public bool IsKeyedService(Type serviceType, object? serviceKey) => this.dependencyResolver.CanResolve(serviceType, serviceKey);
#endif
}