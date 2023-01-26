using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using Stashbox;
using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Stashbox related <see cref="IServiceCollection"/> extensions.
/// </summary>
public static partial class StashboxServiceCollectionExtensions
{
    /// <summary>
    /// Registers a transient service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, Func<RegistrationConfigurator<TService, TImplementation>, RegistrationConfigurator<TService, TImplementation>> configurator)
        where TImplementation : class, TService
        where TService : class
    {
        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), 
            new StashboxServiceDescriptor(container => container.Register<TService, TImplementation>(config =>
            {
                config.WithTransientLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a transient service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTransient(this IServiceCollection services, Type serviceType, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(serviceType, nameof(serviceType));
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), 
            new StashboxServiceDescriptor(container => container.Register(serviceType, implementationType, config =>
            {
                config.WithTransientLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a transient service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTransient(this IServiceCollection services, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), 
            new StashboxServiceDescriptor(container => container.Register(implementationType, config =>
            {
                config.WithTransientLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a transient service descriptor wrapper with a name into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The service name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, object name)
        where TImplementation : class, TService
        where TService : class
    {
        Shield.EnsureNotNull(name, nameof(name));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor), 
            new StashboxServiceDescriptor(container => container.Register<TService, TImplementation>(config => config.WithTransientLifetime().WithName(name)))));
        return services;
    }

    /// <summary>
    /// Registers a singleton service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, Func<RegistrationConfigurator<TService, TImplementation>, RegistrationConfigurator<TService, TImplementation>> configurator)
        where TImplementation : class, TService
        where TService : class
    {
        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register<TService, TImplementation>(config =>
            {
                config.WithSingletonLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a singleton service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(serviceType, nameof(serviceType));
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register(serviceType, implementationType, config =>
            {
                config.WithSingletonLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a singleton service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSingleton(this IServiceCollection services, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register(implementationType, config =>
            {
                config.WithSingletonLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a singleton service descriptor wrapper with a name into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The service name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, object name)
        where TImplementation : class, TService
        where TService : class
    {
        Shield.EnsureNotNull(name, nameof(name));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.RegisterSingleton<TService, TImplementation>(name))));
        return services;
    }

    /// <summary>
    /// Registers a scoped service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<RegistrationConfigurator<TService, TImplementation>, RegistrationConfigurator<TService, TImplementation>> configurator)
        where TImplementation : class, TService
        where TService : class
    {
        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register<TService, TImplementation>(config =>
            {
                config.WithScopedLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a scoped service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddScoped(this IServiceCollection services, Type serviceType, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(serviceType, nameof(serviceType));
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register(serviceType, implementationType, config =>
            {
                config.WithScopedLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a scoped service descriptor wrapper into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="configurator">The service registration configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddScoped(this IServiceCollection services, Type implementationType, Func<RegistrationConfigurator, RegistrationConfigurator> configurator)
    {
        Shield.EnsureNotNull(implementationType, nameof(implementationType));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.Register(implementationType, config =>
            {
                config.WithScopedLifetime();
                configurator?.Invoke(config);
            }))));
        return services;
    }

    /// <summary>
    /// Registers a scoped service descriptor wrapper with a name into the given <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="name">The service name.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, object name)
        where TImplementation : class, TService
        where TService : class
    {
        Shield.EnsureNotNull(name, nameof(name));

        services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
            new StashboxServiceDescriptor(container => container.RegisterScoped<TService, TImplementation>(name))));
        return services;
    }
}