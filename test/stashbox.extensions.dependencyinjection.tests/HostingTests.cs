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
        using (var host = new HostBuilder()
                   .UseStashbox()
                   .ConfigureContainer<IStashboxContainer>((c, s) =>
                   {
                       s.Register<Foo>();
                   })
                   .ConfigureServices((c, s) =>
                   {
                       s.AddHostedService<Service>();
                   })
                   .Build())
        {
            await host.StartAsync();

            await host.StopAsync();
        }
    }
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