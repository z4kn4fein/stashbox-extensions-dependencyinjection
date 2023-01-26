using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Stashbox.AspNetCore.Testing;
using TestApp;

namespace StashboxBenchmarks;

[MemoryDiagnoser]
public class TestingBenchmark
{
    private readonly WebApplicationFactory<TestApp.Program> webApplicationFactory = new();
    private readonly StashboxWebApplicationFactory<TestApp.Program> stashboxWebApplicationFactory = new();
    private readonly ServiceDescriptor descriptor = new(typeof(IA), typeof(A), ServiceLifetime.Singleton);
    
    [Benchmark(Baseline = true)]
    public void WebApplicationFactory_CreateClient()
    {
        webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Add(descriptor);
            });
        }).CreateClient();
    }
    
    [Benchmark]
    public void StashboxWebApplicationFactory_StashClient()
    {
        stashboxWebApplicationFactory.StashClient((services, _) =>
        {
            services.Add(descriptor);
        });
    }
}