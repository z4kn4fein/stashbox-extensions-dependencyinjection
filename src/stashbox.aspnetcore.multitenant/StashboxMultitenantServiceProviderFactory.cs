using Microsoft.Extensions.DependencyInjection;
using Stashbox.Extensions.DependencyInjection;
using Stashbox.Multitenant;
using System;

namespace Stashbox.AspNetCore.Multitenant;

/// <summary>
/// Represents an <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation based on <see cref="ITenantDistributor"/>
/// </summary>
public class StashboxMultitenantServiceProviderFactory : IServiceProviderFactory<IStashboxContainer>
{
    private readonly IStashboxContainer rootContainer;

    /// <summary>
    /// Constructs a <see cref="StashboxMultitenantServiceProviderFactory"/>.
    /// </summary>
    /// <param name="rootContainer">The root container.</param>
    public StashboxMultitenantServiceProviderFactory(IStashboxContainer rootContainer)
    {
        this.rootContainer = rootContainer;
    }

    /// <inheritdoc />
    public IStashboxContainer CreateBuilder(IServiceCollection services)
    {
        var container = services.CreateBuilder(this.rootContainer);
        container.RegisterInstance(this.rootContainer);
        container.ReMap<IServiceScopeFactory>(c => c.WithFactory(r => new StashboxServiceScopeFactory(r)));
        return this.rootContainer;
    }

    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(IStashboxContainer containerBuilder) =>
        new StashboxServiceProvider(containerBuilder);
}