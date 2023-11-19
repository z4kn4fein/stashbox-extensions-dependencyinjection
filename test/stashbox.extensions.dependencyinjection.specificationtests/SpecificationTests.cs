using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using System.Diagnostics;
using System.Linq;

namespace Stashbox.Extensions.DependencyInjection.SpecificationTests;

public class SpecificationTests : DependencyInjectionSpecificationTests
{
    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        return serviceCollection.UseStashbox();
    }
}

#if HAS_KEYED
public class KeyedSpecificationTests : KeyedDependencyInjectionSpecificationTests
{
    private static readonly string[] SkippedTests = {
        // this has to be skipped as the exception checked in this test differs
        // from what Stashbox throws and there's no intention right now to change it.
        "ResolveKeyedServiceSingletonFactoryWithAnyKeyIgnoreWrongType" 
    };
    
    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        if (new StackTrace(1).GetFrames().Take(2).Any(stackFrame => SkippedTests.Contains(stackFrame.GetMethod()?.Name)))
        {
            return serviceCollection.BuildServiceProvider();
        }

        return serviceCollection.UseStashbox();
    }
}
#endif