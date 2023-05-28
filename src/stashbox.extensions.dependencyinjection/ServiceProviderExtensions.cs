using System;
using System.Collections.Generic;
using Stashbox;
using Stashbox.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>Gets the named service object of the specified type.</summary>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>A service object of type <paramref name="serviceType" /> or null if there is no such service.</returns>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static object? GetService(this IServiceProvider provider, Type serviceType, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.ResolveOrDefault(serviceType, name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }
    
    /// <summary>
    /// Get named service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static T? GetService<T>(this IServiceProvider provider, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.Resolve<T>(name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }
    
    /// <summary>
    /// Get named service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>A service object of type <paramref name="serviceType"/>.</returns>
    /// <exception cref="System.InvalidOperationException">There is no service of type <paramref name="serviceType"/> with the given <paramref name="name"/>.</exception>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static object GetRequiredService(this IServiceProvider provider, Type serviceType, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.Resolve(serviceType, name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }
    
    /// <summary>
    /// Get named service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>A service object of type <typeparamref name="T"/>.</returns>
    /// <exception cref="System.InvalidOperationException">There is no service of type <typeparamref name="T"/> with the given <paramref name="name"/>.</exception>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static T GetRequiredService<T>(this IServiceProvider provider, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.Resolve<T>(name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }
    
    /// <summary>
    /// Get an enumeration of named services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>An enumeration of services of type <typeparamref name="T"/>.</returns>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static IEnumerable<T> GetServices<T>(this IServiceProvider provider, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.ResolveAll<T>(name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }

    /// <summary>
    /// Get an enumeration of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <param name="name">The name of the service.</param>
    /// <returns>An enumeration of services of type <paramref name="serviceType"/>.</returns>
    /// <exception cref="NotSupportedException">The service provider is not a <see cref="StashboxServiceProvider"/>.</exception>
    public static IEnumerable<object?> GetServices(this IServiceProvider provider, Type serviceType, object name)
    {
        if (provider is StashboxServiceProvider stashboxServiceProvider)
            return stashboxServiceProvider.DependencyResolver.ResolveAll(serviceType, name);

        throw new NotSupportedException("Only a StashboxServiceProvider can serve named resolution requests.");
    }
}