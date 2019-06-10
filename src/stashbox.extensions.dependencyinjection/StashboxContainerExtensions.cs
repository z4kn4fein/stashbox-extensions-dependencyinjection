using System;

namespace Stashbox.Extensions.Dependencyinjection
{
    internal static class StashboxContainerExtensions
    {
        public static IServiceProvider GetServiceProvider(this IStashboxContainer container) =>
#if HAS_SERVICEPROVIDER
            container;
#else
            container.Resolve<IServiceProvider>();
#endif
    }
}
