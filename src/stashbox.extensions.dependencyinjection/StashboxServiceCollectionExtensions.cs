using Microsoft.Extensions.DependencyInjection.Extensions;
using Stashbox;
using Stashbox.Configuration;
using Stashbox.Extensions.DependencyInjection;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Stashbox related <see cref="IServiceCollection"/> extensions.
/// </summary>
public static partial class StashboxServiceCollectionExtensions
{
    /// <summary>
    /// Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> with a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddStashbox(this IServiceCollection services, Action<IStashboxContainer>? configure = null) =>
        services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(configure)));

    /// <summary>
    /// Replaces the default <see cref="IServiceProviderFactory{TContainerBuilder}"/> with a factory which uses Stashbox as the default <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddStashbox(this IServiceCollection services, IStashboxContainer container) =>
        services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(container)));

    /// <summary>
    /// Registers the services from the <paramref name="serviceCollection"/> and creates a service provider which uses Stashbox.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
    /// <returns>The configured <see cref="IServiceProvider"/> instance.</returns>
    public static IServiceProvider UseStashbox(this IServiceCollection serviceCollection, Action<IStashboxContainer>? configure = null) =>
        new StashboxServiceProvider(serviceCollection.CreateBuilder(configure));


    /// <summary>
    /// Registers the services from the <paramref name="serviceCollection"/> and creates a service provider which uses Stashbox.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
    /// <returns>The configured <see cref="IServiceProvider"/> instance.</returns>
    public static IServiceProvider UseStashbox(this IServiceCollection serviceCollection, IStashboxContainer container) =>
        new StashboxServiceProvider(serviceCollection.CreateBuilder(container));

    /// <summary>
    /// Registers the services from the <paramref name="serviceCollection"/> and returns with the prepared <see cref="IStashboxContainer"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="configure">The callback action to configure the internal <see cref="IStashboxContainer"/>.</param>
    /// <returns>The configured <see cref="IStashboxContainer"/> instance.</returns>
    public static IStashboxContainer CreateBuilder(this IServiceCollection serviceCollection, Action<IStashboxContainer>? configure = null) =>
        PrepareContainer(serviceCollection, configure);

    /// <summary>
    /// Registers the services from the <paramref name="serviceCollection"/> and returns with the prepared <see cref="IStashboxContainer"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
    /// <returns>The configured <see cref="IStashboxContainer"/> instance.</returns>
    public static IStashboxContainer CreateBuilder(this IServiceCollection serviceCollection, IStashboxContainer container) =>
        PrepareContainer(serviceCollection, null, container);

    /// <summary>
    /// Registers the given services into the container.
    /// </summary>
    /// <param name="container">The <see cref="IStashboxContainer"/>.</param>
    /// <param name="services">The service descriptors.</param>
    public static void RegisterServiceDescriptors(this IStashboxContainer container, IEnumerable<ServiceDescriptor> services)
    {
        foreach (var descriptor in services)
        {
            if (descriptor.ImplementationInstance is StashboxServiceDescriptor stashboxServiceDescriptor)
            {
                stashboxServiceDescriptor.ConfigurationAction?.Invoke(container);
                continue;
            }

            var lifetime = ChooseLifetime(descriptor.Lifetime);

            if (descriptor.ImplementationType != null)
                container.Register(descriptor.ServiceType,
                    descriptor.ImplementationType,
                    context => context.WithLifetime(lifetime));
            else if (descriptor.ImplementationFactory != null)
                container.Register(descriptor.ServiceType,
                    context => context
                        .WithFactory(descriptor.ImplementationFactory)
                        .WithLifetime(lifetime));
            else if (descriptor.ImplementationInstance != null)
                container.RegisterInstance(descriptor.ImplementationInstance, descriptor.ServiceType);
        }
    }

    private static LifetimeDescriptor ChooseLifetime(ServiceLifetime serviceLifetime) =>
        serviceLifetime switch
        {
            ServiceLifetime.Scoped => Lifetimes.Scoped,
            ServiceLifetime.Singleton => Lifetimes.Singleton,
            ServiceLifetime.Transient => Lifetimes.Transient,
            _ => throw new ArgumentOutOfRangeException(nameof(serviceLifetime))
        };

    private static IStashboxContainer PrepareContainer(IServiceCollection services,
        Action<IStashboxContainer>? configure = null, IStashboxContainer? stashboxContainer = null)
    {
        var container = stashboxContainer ?? new StashboxContainer();

        container.Configure(config => config
            .WithDisposableTransientTracking()
            .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));

        configure?.Invoke(container);

        container.RegisterInstance<IServiceScopeFactory>(new StashboxServiceScopeFactory(container));
        container.Register<IServiceProvider, StashboxServiceProvider>(c => c
                .WithFactory(r => new StashboxServiceProvider(r))
#if HAS_IS_SERVICE
                .AsServiceAlso<IServiceProviderIsService>()
#endif
        );

        container.RegisterServiceDescriptors(services);

        return container;
    }
}