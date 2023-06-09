using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.AspNetCore.Multitenant.Tests;

public class MultitenantTests
{
    [Fact]
    public async Task MultitenantTests_Works()
    {
        var configureCalled = false;
        var d = new D();
        {
            using var host = await new HostBuilder()
                .UseStashboxMultitenant<TestTenantIdExtractor>(c =>
                {
                    c.RootContainer.RegisterScoped<IA, C>();
                    c.ConfigureTenant("A", cont => cont.RegisterScoped<IA, A>());
                    c.ConfigureTenant("B", cont => cont.RegisterScoped<IA, B>());
                    c.ConfigureTenant("D", cont => cont.RegisterInstance<IA>(d));
                })
                .ConfigureContainer<IStashboxContainer>(c =>
                {
                    Assert.IsType<StashboxContainer>(c);
                    configureCalled = true;
                })
                .ConfigureWebHost(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<TestStartup>();
                })
                .StartAsync();

            var client = host.GetTestClient();

            await this.AssertResult(client, "A", "A");
            await this.AssertResult(client, "B", "B");
            await this.AssertResult(client, "D", "D");

            Assert.False(d.Disposed);

            await this.AssertResult(client, "NONEXISTING", "C");
        }

        Assert.Equal(1, A.DisposedCount);
        Assert.True(d.Disposed);
        Assert.True(configureCalled);
        Assert.True(TestStartup.ConfigureCalled);
    }

    private async Task AssertResult(HttpClient client, string tenantId, string expectedResult)
    {
        using var request1 = new HttpRequestMessage(HttpMethod.Get, "api/test/value");
        request1.Headers.Add(TestTenantIdExtractor.TENANT_HEADER, tenantId);
        using var response1 = await client.SendAsync(request1);
        response1.EnsureSuccessStatusCode();
        Assert.Equal(expectedResult, await response1.Content.ReadAsStringAsync());
    }
}

[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IA testDependency;

    public TestController(IA testDependency)
    {
        this.testDependency = testDependency;
    }

    [HttpGet("value")]
    public string GetValue()
    {
        return this.testDependency.GetType().Name;
    }
}

public class TestTenantIdExtractor : ITenantIdExtractor
{
    public const string TENANT_HEADER = "TENANT-ID";

    public Task<object> GetTenantIdAsync(HttpContext context)
    {
        return Task.FromResult<object>(!context.Request.Headers.TryGetValue(TENANT_HEADER, out var value) 
            ? null 
            : value.First());
    }
}

public class TestStartup
{
    public static bool ConfigureCalled = false;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(TestStartup).Assembly);
    }

    public void ConfigureContainer(IStashboxContainer container)
    {
        Assert.IsType<StashboxContainer>(container);
        ConfigureCalled = true;
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting().UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public interface IA { }

public class A : IA, IDisposable
{
    public static int DisposedCount = 0;
    
    public void Dispose()
    {
        Interlocked.Increment(ref DisposedCount);
    }
}

public class B : IA { }

public class C : IA { }

class D : IA, IAsyncDisposable
{
    public bool Disposed { get; private set; }

    public ValueTask DisposeAsync()
    {
        if (this.Disposed)
            throw new ObjectDisposedException(nameof(C));

        this.Disposed = true;

        return new ValueTask(Task.CompletedTask);
    }
}