using System;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.AspNetCore.Multitenant;

/// <summary>
/// Describes the options required for multi-tenant configuration.
/// </summary>
public class StashboxMultitenantOptions
{
    /// <summary>
    /// The root container.
    /// </summary>
    public IStashboxContainer RootContainer { get; }

    /// <summary>
    /// Creates a <see cref="StashboxMultitenantOptions"/>.
    /// </summary>
    /// <param name="rootContainer">The root container.</param>
    public StashboxMultitenantOptions(IStashboxContainer rootContainer)
    {
        this.RootContainer = rootContainer;
    }

    /// <summary>
    /// Adds a tenant with a specified service configuration to the root container.
    /// </summary>
    /// <param name="tenantId">The identifier of the tenant.</param>
    /// <param name="tenantConfig">The service configuration of the tenant.</param>
    /// <param name="attachTenantToRoot">If true, the new tenant will be attached to the lifecycle of the root container. When the root is being disposed, the tenant will be disposed with it.</param>
    /// <returns>A service configurator used to configure services for the tenant.</returns>
    public ITenantServiceConfigurator ConfigureTenant(object tenantId, Action<IStashboxContainer>? tenantConfig = null, bool attachTenantToRoot = true)
    {
        var child = this.RootContainer.CreateChildContainer(tenantId, tenantConfig, attachTenantToRoot);
        return new TenantServiceConfigurator(child);
    }
    
    /// <summary>
    /// Registers services into the root container from a <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="configuration">The configuration delegate.</param>
    public void ConfigureRootServices(Action<IServiceCollection> configuration)
    {
        var collection = new ServiceCollection();
        configuration(collection);
        this.RootContainer.RegisterServiceDescriptors(collection);
    }
}

/// <summary>
/// Describes a utility class to register services into a tenant container from a <see cref="IServiceCollection"/>.
/// </summary>
public interface ITenantServiceConfigurator
{
    /// <summary>
    /// Registers services into the tenant from a <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="configuration">The configuration delegate.</param>
    void ConfigureServices(Action<IServiceCollection> configuration);
}

internal class TenantServiceConfigurator(IStashboxContainer tenantContainer) : ITenantServiceConfigurator
{
    public void ConfigureServices(Action<IServiceCollection> configuration)
    {
        var collection = new ServiceCollection();
        configuration(collection);
        tenantContainer.RegisterServiceDescriptors(collection);
    }
}