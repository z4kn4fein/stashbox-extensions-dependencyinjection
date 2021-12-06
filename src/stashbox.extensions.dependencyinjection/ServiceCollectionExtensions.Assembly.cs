using Stashbox;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Registration.Fluent;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Stashbox related <see cref="IServiceCollection"/> extensions.
    /// </summary>
    public static partial class StashboxServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services from an assembly of the given type.
        /// </summary>
        /// <typeparam name="TService">The type used to access the assembly.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="selector">The type selector. Used to filter which types should be excluded/included in the registration process.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter which interface or base types the implementation should be mapped to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">Configurator action for the registered types.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ScanAssemblyOf<TService>(this IServiceCollection services,
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null)
            where TService : class
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.RegisterAssemblyContaining< TService>(selector,
                serviceTypeSelector,
                registerSelf,
                configurator))));
            return services;
        }

        /// <summary>
        /// Registers services from an assembly of the given type.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="type">The type used to access the assembly.</param>
        /// <param name="selector">The type selector. Used to filter which types should be excluded/included in the registration process.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter which interface or base types the implementation should be mapped to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">Configurator action for the registered types.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ScanAssemblyOf(this IServiceCollection services, 
            Type type,
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null)
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.RegisterAssemblyContaining(type,
                selector,
                serviceTypeSelector,
                registerSelf,
                configurator))));
            return services;
        }

        /// <summary>
        /// Registers services from an assembly.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly holding the types to register.</param>
        /// <param name="selector">The type selector. Used to filter which types should be excluded/included in the registration process.</param>
        /// <param name="serviceTypeSelector">The service type selector. Used to filter which interface or base types the implementation should be mapped to.</param>
        /// <param name="registerSelf">If it's true the types will be registered to their own type too.</param>
        /// <param name="configurator">Configurator action for the registered types.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ScanAssembly(this IServiceCollection services,
            Assembly assembly,
            Func<Type, bool> selector = null,
            Func<Type, Type, bool> serviceTypeSelector = null,
            bool registerSelf = true,
            Action<RegistrationConfigurator> configurator = null)
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.RegisterAssembly(assembly,
                selector,
                serviceTypeSelector,
                registerSelf,
                configurator))));
            return services;
        }

        /// <summary>
        /// Scans the given assembly for <see cref="ICompositionRoot"/> implementations and invokes their <see cref="ICompositionRoot.Compose"/> method.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assembly">The assembly to scan.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ComposeAssembly(this IServiceCollection services, Assembly assembly)
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.ComposeAssembly(assembly))));
            return services;
        }

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given root.
        /// </summary>
        /// <typeparam name="TCompositionRoot">>The type of an <see cref="ICompositionRoot"/> implementation.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="compositionRootArguments">Optional composition root constructor argument overrides.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ComposeBy<TCompositionRoot>(this IServiceCollection services, params object[] compositionRootArguments)
             where TCompositionRoot : class, ICompositionRoot
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.ComposeBy<TCompositionRoot>(compositionRootArguments))));
            return services;
        }

        /// <summary>
        /// Composes services by calling the <see cref="ICompositionRoot.Compose"/> method of the given root.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="compositionRootType">The type of an <see cref="ICompositionRoot"/> implementation.</param>
        /// <param name="compositionRootArguments">Optional composition root constructor argument overrides.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection ComposeBy(this IServiceCollection services, Type compositionRootType, params object[] compositionRootArguments)
        {
            services.Add(new ServiceDescriptor(typeof(StashboxServiceDescriptor),
                new StashboxServiceDescriptor(container => container.ComposeBy(compositionRootType, compositionRootArguments))));
            return services;
        }
    }
}
