using Stashbox.Extensions.DependencyInjection;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Stashbox related <see cref="IServiceCollection"/> extensions.
/// </summary>
public static partial class StashboxServiceCollectionExtensions
{
    /// <summary>
    /// Registers a decorator service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">Optional service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection Decorate<TService, TImplementation>(this IServiceCollection services, 
        Action<DecoratorConfigurator<TService, TImplementation>>? configurator = null)
        where TImplementation : class, TService
        where TService : class
    {
        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), new StashboxServiceDescriptor(container => container.RegisterDecorator(configurator))));
        return services;
    }

    /// <summary>
    /// Registers a decorator service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">Optional service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection Decorate<TImplementation>(this IServiceCollection services, 
        Action<DecoratorConfigurator<TImplementation, TImplementation>>? configurator = null)
        where TImplementation : class
    {
        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), new StashboxServiceDescriptor(container => container.RegisterDecorator<TImplementation>(configurator))));
        return services;
    }

    /// <summary>
    /// Registers a decorator service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">Optional service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection Decorate(this IServiceCollection services, Type serviceType, 
        Type implementationType, Action<DecoratorConfigurator>? configurator = null)
    {
        Shield.EnsureNotNull(serviceType, nameof(serviceType));
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), new StashboxServiceDescriptor(container => container.RegisterDecorator(serviceType, implementationType, configurator))));
        return services;
    }

    /// <summary>
    /// Registers a decorator service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">Optional service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection Decorate(this IServiceCollection services, 
        Type implementationType, Action<DecoratorConfigurator>? configurator = null)
    {
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), new StashboxServiceDescriptor(container => container.RegisterDecorator(implementationType, configurator))));
        return services;
    }
}