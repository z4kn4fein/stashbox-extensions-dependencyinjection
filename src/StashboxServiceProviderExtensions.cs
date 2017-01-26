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
        /// Adds the stashbox container as the default service provider.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configure">An <see cref="IStashboxContainer"/> configuration callback.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider UseStashboxServiceProvider(this IServiceCollection services, Action<IStashboxContainer> configure = null)
        {
            var container = new StashboxContainer(config => 
                config.WithDisposableTransientTracking()
                .WithCircularDependencyTracking()
                .WithParentContainerResolution()
                .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)
                .WithDependencySelectionRule(Rules.DependencySelection.PreferLastRegistered)
                .WithEnumerableOrderRule(Rules.EnumerableOrder.PreserveOrder));
            
            container.RegisterInstance<IStashboxContainer>(container);
            container.RegisterScoped<IServiceScopeFactory, StashboxServiceScopeFactory>();
            container.RegisterScoped<IServiceProvider, StashboxServiceProvider>();

            container.RegisterServiceDescriptors(services);

            configure?.Invoke(container);

            return container.Resolve<IServiceProvider>();
        }

        /// <summary>
        /// Configures stashbox container through a configurator class.
        /// </summary>
        /// <typeparam name="TConfigurator">The type which will configure the container.</typeparam>
        /// <param name="serviceProvider">The <see cref="StashboxServiceProvider"/> instance to extend.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider ConfigureStashboxServiceProvider<TConfigurator>(this IServiceProvider serviceProvider)
            where TConfigurator : class
        {
            if (serviceProvider.GetType() != typeof(StashboxServiceProvider))
                throw new ArgumentException("The given service provider is not a stashbox service provider.");

            var container = serviceProvider.GetService(typeof(IStashboxContainer)) as IStashboxContainer;
            container?.RegisterType<TConfigurator>();
            container?.Resolve<TConfigurator>();
            
            return serviceProvider;
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
                         .WithScopeManagement()
                         .Register();
            else
                container.PrepareType(descriptor.ServiceType).WithFactory(() => descriptor.ImplementationInstance)
                         .WithScopeManagement()
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
                container.PrepareType(descriptor.ServiceType).WithFactory(() => descriptor.ImplementationInstance)
                         .WithLifetime(new SingletonLifetime())
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
                container.PrepareType(descriptor.ServiceType).WithFactory(() => descriptor.ImplementationInstance)
                         .Register();
        }
    }
}
