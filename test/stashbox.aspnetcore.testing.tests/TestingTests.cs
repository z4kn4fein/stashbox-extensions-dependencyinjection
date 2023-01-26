using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TestApp;
using Xunit;

namespace Stashbox.AspNetCore.Testing.Tests;

public class TestingTests
{
    [Fact]
    public async Task Stashbox_WebAppFactory()
    {
        var client = new StashboxWebApplicationFactory<Program>().StashClient((_, _) =>
        {
        });

        var response = await client.GetAsync("api/test/value");
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("A", body);
    }
    
    [Fact]
    public async Task Stashbox_WebAppFactory_Override()
    {
        var client = new StashboxWebApplicationFactory<Program>().StashClient((services, _) =>
        {
            services.AddSingleton<IA, B>();
        });

        var response = await client.GetAsync("api/test/value");
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("B", body);
    }
    
    [Fact]
    public async Task Stashbox_WebAppFactory_Dispose()
    {
        var c = new C();
        var d = new D();
        var factory = new StashboxWebApplicationFactory<Program>();
        var client1 = factory.StashClient((services, _) =>
        {
            services.AddSingleton<IA>(c);
        });

        var response = await client1.GetAsync("api/test/value");
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("C", body);
        
        var client2 = factory.StashClient((services, _) =>
        {
            services.AddSingleton<IA>(d);
        });

        response = await client2.GetAsync("api/test/value");
        body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("D", body);
        
        factory.Dispose();
        
        Assert.True(c.Disposed);
        Assert.True(d.Disposed);
    }
    
    [Fact]
    public async Task Stashbox_WebAppFactory_DisposeAsync()
    {
        var c = new C();
        var d = new D();
        var factory = new StashboxWebApplicationFactory<Program>();
        var client1 = factory.StashClient((services, _) =>
        {
            services.AddSingleton<IA>(c);
        });

        var response = await client1.GetAsync("api/test/value");
        var body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("C", body);
        
        var client2 = factory.StashClient((services, _) =>
        {
            services.AddSingleton<IA>(d);
        });

        response = await client2.GetAsync("api/test/value");
        body = await response.Content.ReadAsStringAsync();
        
        Assert.Equal("D", body);
        
        await factory.DisposeAsync();
        
        Assert.True(c.Disposed);
        Assert.True(d.Disposed);
    }
}