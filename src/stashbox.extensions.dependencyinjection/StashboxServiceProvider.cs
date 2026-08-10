using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stashbox.Extensions.DependencyInjection;

/// <summary>
/// An <see cref="IServiceProvider"/> implementation that uses Stashbox to produce services.
/// </summary>
public class StashboxServiceProvider : IServiceProvider, ISupportRequiredService, IServiceScopeFactory, IServiceScope,
#if HAS_IS_SERVICE
    IServiceProviderIsService,
#endif
#if HAS_KEYED
    IKeyedServiceProvider,
    IServiceProviderIsKeyedService,
#endif
    IAsyncDisposable
{
    private static readonly Type ServiceProviderType = typeof(IServiceProvider);

    /// <summary>
    /// The <see cref="IDependencyResolver"/> instance used by this service provider.
    /// </summary>
    public IDependencyResolver DependencyResolver { get; }

    /// <summary>
    /// Constructs a <see cref="StashboxServiceProvider"/>.
    /// </summary>
    /// <param name="dependencyResolver">A Stashbox <see cref="IDependencyResolver"/> implementation.</param>
    public StashboxServiceProvider(IDependencyResolver dependencyResolver)
    {
        this.DependencyResolver = dependencyResolver;
    }
    
    /// <inheritdoc />
    public IServiceProvider ServiceProvider => this;

    /// <inheritdoc />
    public IServiceScope CreateScope() =>
        new StashboxServiceProvider(this.DependencyResolver.BeginScope());

    /// <inheritdoc />
    public object? GetService(Type serviceType) =>
        serviceType == ServiceProviderType ? this : this.DependencyResolver.ResolveOrDefault(serviceType);
    

    /// <inheritdoc />
    public object GetRequiredService(Type serviceType) =>
        serviceType == ServiceProviderType ? this : this.DependencyResolver.Resolve(serviceType);

    /// <inheritdoc />
    public void Dispose() => this.DependencyResolver.Dispose();

    /// <inheritdoc />
    public ValueTask DisposeAsync() => this.DependencyResolver.DisposeAsync();

#if HAS_IS_SERVICE
    /// <inheritdoc />
    public bool IsService(Type serviceType) => this.DependencyResolver.CanResolve(serviceType);
#endif

#if HAS_KEYED
    /// <inheritdoc />
    public object? GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (serviceKey == KeyedService.AnyKey && (!serviceType.IsGenericType || serviceType.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
            throw new InvalidOperationException(
                "KeyedService.AnyKey cannot be used to resolve a single service.");
        
        return this.DependencyResolver.ResolveOrDefault(serviceType, serviceKey);
    }

    /// <inheritdoc />
    public object GetRequiredKeyedService(Type serviceType, object? serviceKey) =>
        this.DependencyResolver.Resolve(serviceType, serviceKey);

    /// <inheritdoc />
    public bool IsKeyedService(Type serviceType, object? serviceKey) =>
        this.DependencyResolver.CanResolve(serviceType, serviceKey);
#endif
}