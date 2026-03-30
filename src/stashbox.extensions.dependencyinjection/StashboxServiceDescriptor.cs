using System;
using Microsoft.Extensions.DependencyInjection;

namespace Stashbox.Extensions.DependencyInjection;

internal class StashboxServiceDescriptor(Action<IStashboxContainer> configurationAction)
    : ServiceDescriptor(DescriptorType, DescriptorType, ServiceLifetime.Transient)
{
    private static readonly Type DescriptorType = typeof(StashboxServiceDescriptor);
    
    public Action<IStashboxContainer> ConfigurationAction { get; } = configurationAction;
}