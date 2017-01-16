using Microsoft.Extensions.DependencyInjection;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    /// <summary>
    /// Represents an <see cref="IServiceProvider"/> implementation based on the <see cref="StashboxContainer"/>
    /// </summary>
    public class StashboxServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        private readonly IStashboxContainer stashboxContainer;

        /// <summary>
        /// Constructs a <see cref="StashboxServiceProvider"/>
        /// </summary>
        /// <param name="stashboxContainer"></param>
        public StashboxServiceProvider(IStashboxContainer stashboxContainer)
        {
            Shield.EnsureNotNull(stashboxContainer, nameof(stashboxContainer));

            this.stashboxContainer = stashboxContainer;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.stashboxContainer.IsRegistered(serviceType) ? this.stashboxContainer.Resolve(serviceType) : null;
        }

        /// <inheritdoc />
        public object GetRequiredService(Type serviceType)
        {
            return this.stashboxContainer.Resolve(serviceType);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.stashboxContainer.Dispose();
        }
    }
}
