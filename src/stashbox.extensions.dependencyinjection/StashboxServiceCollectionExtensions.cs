using Microsoft.Extensions.DependencyInjection.Extensions;
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
            services.CreateBuilder(configure).GetServiceProvider();


        /// <summary>
        /// Creates a service provider using <see cref="IStashboxContainer"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        /// <returns>The configured <see cref="StashboxServiceProvider"/> instance.</returns>
        public static IServiceProvider UseStashbox(this IServiceCollection services, IStashboxContainer container) =>
            services.CreateBuilder(container).GetServiceProvider();

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
                var lifetime = ChooseLifetime(descriptor.Lifetime);

                if (descriptor.ImplementationType != null)
                    container.Register(descriptor.ServiceType, 
                        descriptor.ImplementationType, 
                        context => context.WithLifetime(lifetime));
                else if (descriptor.ImplementationFactory != null)
                    container.Register(descriptor.ServiceType, 
                        context => context
#if HAS_SERVICEPROVIDER
                        .WithFactory(descriptor.ImplementationFactory)
#else
                        .WithFactory(c => descriptor.ImplementationFactory(c.Resolve<IServiceProvider>()))
#endif
                        .WithLifetime(lifetime));
                else
                    container.RegisterInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
            }
        }

        private static ILifetime ChooseLifetime(ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    return new ScopedLifetime();
                case ServiceLifetime.Singleton:
                    return new SingletonLifetime();
                case ServiceLifetime.Transient:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
            }
        }

        private static IStashboxContainer PrepareContainer(IServiceCollection services,
            Action<IStashboxContainer> configure = null, IStashboxContainer stashboxContainer = null)
        {
            var container = stashboxContainer ?? new StashboxContainer(config =>
                config.WithDisposableTransientTracking()
                .WithUniqueRegistrationIdentifiers());

            configure?.Invoke(container);

            container.RegisterInstanceAs<IServiceScopeFactory>(new StashboxServiceScopeFactory(container));
#if HAS_SERVICEPROVIDER
            container.RegisterInstanceAs<IServiceProvider>(container);
#else
            container.RegisterInstanceAs<IServiceProvider>(new StashboxServiceProvider(container));
#endif

            container.RegisterServiceDescriptors(services);

            return container;
        }
    }
}
