using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TestApp;
using Xunit;

namespace Stashbox.AspNetCore.Testing.Tests;

public class FixtureTests : IClassFixture<StashboxWebApplicationFactory<Program>>
{
    private readonly StashboxWebApplicationFactory<Program> factory;

    public FixtureTests(StashboxWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }
    
    [Fact]
    public async Task Fixture_WebAppFactory()
    {
        var client = this.factory.StashClient((services, _) =>
        {
            services.AddSingleton<IA, B>();
        });

        var response = await client.GetAsync("api/test/value");
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("B", body);
    }
    
    [Fact]
    public void Derived_Factory()
    {
        var container = new StashboxContainer();
        var derivedFactory = new TestDerivedFactory(container);
        
        Assert.Equal(container.ContainerContext, derivedFactory.TenantDistributor.ContainerContext);
        Assert.True(derivedFactory.TenantDistributor.ContainerContext.ContainerConfiguration.ReBuildSingletonsInChildContainerEnabled);
    }
    
    private class TestDerivedFactory : StashboxWebApplicationFactory<Program>
    {
        public TestDerivedFactory(IStashboxContainer container) : base (container)
        { }
    }
}