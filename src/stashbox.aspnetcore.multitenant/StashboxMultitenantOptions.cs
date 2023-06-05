using System;

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
    public void ConfigureTenant(object tenantId, Action<IStashboxContainer> tenantConfig, bool attachTenantToRoot = true)
    {
        this.RootContainer.CreateChildContainer(tenantId, tenantConfig, attachTenantToRoot);
    }
}