using System;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.DependencyInjection;

internal class StashboxServiceDescriptor : ServiceDescriptor
{
    private static readonly Type DescriptorType = typeof(StashboxServiceDescriptor);
    
    public Action<IStashboxContainer> ConfigurationAction { get; }

    public StashboxServiceDescriptor(Action<IStashboxContainer> configurationAction) : base(DescriptorType, DescriptorType, ServiceLifetime.Transient)
    {
        ConfigurationAction = configurationAction;
    }
}