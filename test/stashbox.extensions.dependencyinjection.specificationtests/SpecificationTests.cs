using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Specification;
using System;

namespace Stashbox.Extensions.DependencyInjection.SpecificationTests;

public class SpecificationTests : DependencyInjectionSpecificationTests
{
    protected override IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
    {
        return serviceCollection.UseStashbox();
    }
}