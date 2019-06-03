﻿using Microsoft.Extensions.DependencyInjection.Extensions;
using Stashbox;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Holds the extension methods for the <see cref="IServiceProvider"/> implementation of the stashbox container.
    /// </summary>
    public static class StashboxServiceCollectionExtensions
    {
        /// <summary>
        /// Adds <see cref="IStashboxContainer"/> as an <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStashbox(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(configure)));

        /// <summary>
        /// Adds <see cref="IStashboxContainer"/> as an <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStashbox(this IServiceCollection services, IStashboxContainer container) =>
            services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(container)));

        /// <summary>
        /// Creates a service provider using <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            services.CreateBuilder(configure).Resolve<IServiceProvider>();

        /// <summary>
        /// Creates a service provider using <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection services, IStashboxContainer container) =>
            services.CreateBuilder(container).Resolve<IServiceProvider>();

        /// <summary>
        /// Creates an <see cref="IStashboxContainer"/> configured to using as an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The configured <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer CreateBuilder(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            PrepareContainer(services, configure);

        /// <summary>
        /// Creates an <see cref="IStashboxContainer"/> configured to using as an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The configured <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer CreateBuilder(this IServiceCollection services, IStashboxContainer container) =>
            PrepareContainer(services, null, container);

        /// <summary>
        /// Registers service descriptors into the container.
        /// </summary>
        /// <param name="container">The <see cref="IStashboxContainer"/>.</param>
        /// <param name="services">The service descriptors.</param>
        public static void RegisterServiceDescriptors(this IDependencyRegistrator container, IEnumerable<ServiceDescriptor> services)
        {
            foreach (var descriptor in services)
            {
                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Scoped:
                        RegisterScopedDescriptor(container, descriptor);
                        break;
                    case ServiceLifetime.Singleton:
                        RegisterSingletonDescriptor(container, descriptor);
                        break;
                    case ServiceLifetime.Transient:
                        RegisterTransientDescriptor(container, descriptor);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(descriptor.Lifetime));
                }
            }
        }

        private static IStashboxContainer PrepareContainer(IServiceCollection services,
            Action<IStashboxContainer> configure = null, IStashboxContainer stashboxContainer = null)
        {
            var container = stashboxContainer ?? new StashboxContainer(config =>
                config.WithDisposableTransientTracking()
                .WithUniqueRegistrationIdentifiers());

            configure?.Invoke(container);

            container.RegisterSingleton<IServiceScopeFactory, StashboxServiceScopeFactory>();
            container.RegisterSingleton<IServiceProvider, StashboxServiceProvider>();

            container.RegisterServiceDescriptors(services);

            return container;
        }

        private static void RegisterScopedDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.RegisterScoped(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.Register(descriptor.ServiceType, context => context
#if HAS_SERVICEPROVIDER
                    .WithFactory(descriptor.ImplementationFactory)
#else
                    .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
#endif
                    .WithLifetime(new ScopedLifetime()));
            else
                container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
        }

        private static void RegisterSingletonDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.RegisterSingleton(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.Register(descriptor.ServiceType, context => context
#if HAS_SERVICEPROVIDER
                    .WithFactory(descriptor.ImplementationFactory)
#else
                    .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
#endif
                    .WithLifetime(new SingletonLifetime()));
            else
                container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
        }

        private static void RegisterTransientDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.Register(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.Register(descriptor.ServiceType, context => context
#if HAS_SERVICEPROVIDER
                    .WithFactory(descriptor.ImplementationFactory));
#else
                    .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>())));
#endif
            else
                container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
        }
    }
}
