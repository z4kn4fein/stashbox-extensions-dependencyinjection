using Microsoft.Extensions.DependencyInjection;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Multitenant;
using System;

namespace Stashbox.AspNetCore.Multitenant
{
    /// <summary>
    /// Represents an <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation based on <see cref="ITenantDistributor"/>
    /// </summary>
    public class StashboxMultitenantServiceProviderFactory : IServiceProviderFactory<IStashboxContainer>
    {
        private readonly ITenantDistributor tenantDistributor;

        /// <summary>
        /// Constructs a <see cref="StashboxMultitenantServiceProviderFactory"/>.
        /// </summary>
        /// <param name="tenantDistributor">The tenant distributor.</param>
        public StashboxMultitenantServiceProviderFactory(ITenantDistributor tenantDistributor)
        {
            this.tenantDistributor = tenantDistributor;
        }

        /// <inheritdoc />
        public IStashboxContainer CreateBuilder(IServiceCollection services)
        {
            var container = services.CreateBuilder(this.tenantDistributor);
            container.RegisterInstance(this.tenantDistributor);
            container.ReMap<IServiceScopeFactory>(c => c.WithFactory(r => new StashboxServiceScopeFactory(r)));
            return this.tenantDistributor;
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IStashboxContainer containerBuilder) =>
            new StashboxServiceProvider(containerBuilder);
    }
}
