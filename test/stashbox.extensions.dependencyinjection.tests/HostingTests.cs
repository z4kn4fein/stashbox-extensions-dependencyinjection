using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stashbox.Utils;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Extensions.DependencyInjection.Tests;

public class HostingTests
{
    [Fact]
    public async Task TestStashboxHosting()
    {
        using var host = new HostBuilder()
            .UseStashbox()
            .ConfigureContainer<IStashboxContainer>((c, s) =>
            {
                s.Register<Foo>();
            })
            .ConfigureServices((c, s) =>
            {
                s.AddHostedService<Service>();
            })
            .Build();
        await host.StartAsync();

        await host.StopAsync();
    }
    
    [Fact]
    public async Task DependencyResolverAccessibleOnServiceScope()
    {
        using var host = new HostBuilder()
            .UseStashbox()
            .Build();
        await host.StartAsync();
        
        var factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = factory.CreateScope();

        var resolver = ((StashboxServiceProvider)scope).DependencyResolver;
        resolver.PutInstanceInScope(new Foo());
        
        Assert.NotNull(scope.ServiceProvider.GetService<Foo>());
        
        await host.StopAsync();
    }

#if NET10_0_OR_GREATER
    [Fact]
    public async Task TestHostApplicationBuilder()
    {
        var builder = new HostApplicationBuilder()
            .UseStashbox(c => c.Register<Foo>());
        builder.Services.AddHostedService<Service>();
        using var host = builder.Build();
        await host.StartAsync();
        
        await host.StopAsync();
    }
#endif
    
}

internal class Service : IHostedService
{
    public Service(Foo foo)
    {
        Shield.EnsureNotNull(foo, "foo");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }
}

internal class Foo
{ }