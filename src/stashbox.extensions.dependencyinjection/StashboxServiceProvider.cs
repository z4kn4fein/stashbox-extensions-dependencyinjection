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
    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        // MSDI contract: passing KeyedService.AnyKey to a single-service getter is ambiguous.
        // Matches Microsoft.Extensions.DependencyInjection.ServiceProvider.GetKeyedService.
        if (KeyedService.AnyKey.Equals(serviceKey))
            throw new InvalidOperationException(
                "This service descriptor is keyed. Your service provider may not support keyed services.");

        // MSDI contract: return null when no registration matches the (type, key) pair.
        // The container is configured with WithForceThrowWhenNamedDependencyIsNotResolvable
        // so that [FromKeyedServices] constructor injection fails loudly, but that flag also
        // makes a plain ResolveOrDefault(type, name) throw on miss. Gate the call on CanResolve
        // so a confirmed miss returns null without invoking the throwing path, while anything
        // that could resolve (including the AnyKey/universal-name fallback) still goes through
        // ResolveOrDefault — genuine activation failures propagate as before.
        return this.dependencyResolver.CanResolve(serviceType, serviceKey)
               || this.dependencyResolver.CanResolve(serviceType, KeyedService.AnyKey)
            ? this.dependencyResolver.ResolveOrDefault(serviceType, serviceKey)
            : null;
    }

    /// <inheritdoc />
    public object GetRequiredKeyedService(Type serviceType, object? serviceKey) =>
        this.dependencyResolver.Resolve(serviceType, serviceKey);
    
    /// <inheritdoc />
    public bool IsKeyedService(Type serviceType, object? serviceKey) => this.dependencyResolver.CanResolve(serviceType, serviceKey);
#endif
}