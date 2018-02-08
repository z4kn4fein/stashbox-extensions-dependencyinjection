using Microsoft.Extensions.DependencyInjection;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents an <see cref="IServiceProviderFactory{TContainerBuilder}"/> implementation based on the <see cref="IStashboxContainer"/>
    /// </summary>
    public class StashboxServiceProviderFactory : IServiceProviderFactory<IStashboxContainer>
    {
        private readonly IStashboxContainer container;
        private readonly Action<IStashboxContainer> configure;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProviderFactory"/>
        /// </summary>
        /// <param name="configure">The callback action which can be used to configure the internal <see cref="IStashboxContainer"/>.</param>
        public StashboxServiceProviderFactory(Action<IStashboxContainer> configure = null)
        {
            this.configure = configure;
        }

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProviderFactory"/>
        /// </summary>
        /// <param name="container">An already configured <see cref="IStashboxContainer"/> instance to use.</param>
        public StashboxServiceProviderFactory(IStashboxContainer container)
        {
            this.container = container;
        }

        /// <inheritdoc />
        public IStashboxContainer CreateBuilder(IServiceCollection services) =>
            this.container != null ? services.CreateBuilder(this.container) : services.CreateBuilder(this.configure);

        /// <inheritdoc />
        public IServiceProvider CreateServiceProvider(IStashboxContainer containerBuilder) =>
            containerBuilder.Resolve<IServiceProvider>();
    }
}
