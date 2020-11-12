using Microsoft.Extensions.DependencyInjection;
using Stashbox.Extensions.Dependencyinjection;
using Stashbox.Multitenant;
using System;

namespace Stashbox.AspNetCore.Multitenant
{
    /// <summary>
    /// Represents an <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation based on <see cref="ITenantDistributor"/>
    /// </summary>
    public class StashboxMultitenantServiceProviderFactory : IServiceProviderFactory<ITenantDistributor>
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
        public ITenantDistributor CreateBuilder(IServiceCollection services)
        {
            services.Add(new ServiceDescriptor(typeof(ITenantDistributor), this.tenantDistributor));
            services.CreateBuilder(this.tenantDistributor.RootContainer);
            return this.tenantDistributor;
        }

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(ITenantDistributor containerBuilder) => 
            new StashboxRequiredServiceProvider(containerBuilder.RootContainer);
    }
}
