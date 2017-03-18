using Stashbox;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Infrastructure;
using Stashbox.Lifetime;
using System;
using System.Collections.Generic;
using Stashbox.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Holds the extension methods for the <see cref="IServiceProvider"/> implementation of the stashbox container.
    /// </summary>
    public static class StashboxServiceProviderExtensions
    {
        /// <summary>
        /// Adds <see cref="IStashboxContainer"/> as an <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStashbox(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            services.AddSingleton<IServiceProviderFactory<IStashboxContainer>>(new StashboxServiceProviderFactory(configure));

        /// <summary>
        /// Creates a service provider using <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            services.CreateBuilder(configure).Resolve<IServiceProvider>();

        /// <summary>
        /// Creates an <see cref="IStashboxContainer"/> configured to using as an <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        /// <returns>The configured <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer CreateBuilder(this IServiceCollection services, Action<IStashboxContainer> configure = null) =>
            PrepareContainer(services, configure);

        private static IStashboxContainer PrepareContainer(IServiceCollection services, Action<IStashboxContainer> configure = null)
        {
            var container = new StashboxContainer(config =>
                config.WithDisposableTransientTracking()
                .WithCircularDependencyTracking()
                .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)
                .WithDependencySelectionRule(Rules.DependencySelection.PreferLastRegistered)
                .WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));

            container.RegisterInstance<IStashboxContainer>(container);
            container.RegisterScoped<IServiceScopeFactory, StashboxServiceScopeFactory>();
            container.RegisterScoped<IServiceProvider, StashboxServiceProvider>();

            container.RegisterServiceDescriptors(services);

            configure?.Invoke(container);

            return container;
        }

        private static void RegisterServiceDescriptors(this IDependencyRegistrator container, IEnumerable<ServiceDescriptor> services)
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

        private static void RegisterScopedDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.RegisterScoped(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.PrepareType(descriptor.ServiceType)
                         .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
                         .WithLifetime(new ScopedLifetime())
                         .Register();
            else
                container.PrepareType(descriptor.ServiceType)
                         .WithInstance(descriptor.ImplementationInstance)
                         .Register();
        }

        private static void RegisterSingletonDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.RegisterSingleton(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.PrepareType(descriptor.ServiceType)
                         .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
                         .WithLifetime(new SingletonLifetime())
                         .Register();
            else
                container.PrepareType(descriptor.ServiceType)
                         .WithInstance(descriptor.ImplementationInstance)
                         .Register();
        }

        private static void RegisterTransientDescriptor(IDependencyRegistrator container, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                container.RegisterType(descriptor.ServiceType, descriptor.ImplementationType);
            else if (descriptor.ImplementationFactory != null)
                container.PrepareType(descriptor.ServiceType)
                         .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
                         .Register();
            else
                container.PrepareType(descriptor.ServiceType)
                         .WithInstance(descriptor.ImplementationInstance)
                         .Register();
        }
    }
}
