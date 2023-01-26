using System;

namespace Stashbox.Extensions.Dependencyinjection;

internal class StashboxServiceDescriptor
{
    public Action<IStashboxContainer> ConfigurationAction { get; }

    public StashboxServiceDescriptor(Action<IStashboxContainer> configurationAction)
    {
        ConfigurationAction = configurationAction;
    }
}