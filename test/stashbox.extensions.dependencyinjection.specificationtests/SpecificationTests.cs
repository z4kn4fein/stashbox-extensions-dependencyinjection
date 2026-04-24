using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

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
    private static readonly string[] SkippedTests =
    {
#if NET8_0
        // The specification changed between .NET 8 and .NET 10, this test is passing in the .NET 10 spec version.
        "ResolveKeyedServicesSingletonInstanceWithAnyKey"
#endif
#if NET9_0
        // The specification changed between .NET 9 and .NET 10, this test is passing in the .NET 10 spec version.
        "ResolveKeyedServicesAnyKeyWithAnyKeyRegistration",
        "ResolveKeyedServicesSingletonInstanceWithAnyKey"
#endif
#if NET10_0
        // These tests require to return with the same IEnumerable instance for subsequent keyed collection requests.
        // Stashbox returns a new IEnumerable with the same items for each request. 
        "ResolveWithAnyKeyQuery_Constructor",
        "ResolveWithAnyKeyQuery_Constructor_Duplicates",
        // These tests require the new key inheritance mechanism of the FromKeyedServices attribute.
        // There's no such thing as key inheritance in Stashbox. Even if it has, the way it was implemented in MS.DI
        // makes it nearly impossible to conform to without depending on the whole MS.DI package.
        "ResolveKeyedServiceWithFromServiceKeyAttribute",
        "ResolveKeyedServiceWithFromServiceKeyAttribute_NotFound"
#endif
    };

    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        if (new StackTrace(1).GetFrames().Take(2)
            .Any(stackFrame => SkippedTests.Contains(stackFrame.GetMethod()?.Name)))
        {
            return serviceCollection.BuildServiceProvider();
        }

        return serviceCollection.UseStashbox();
    }
}
#endif