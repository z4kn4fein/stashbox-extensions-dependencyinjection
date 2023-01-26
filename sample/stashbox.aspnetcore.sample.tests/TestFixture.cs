using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Stashbox.AspNetCore.Testing;

namespace Stashbox.AspNetCore.Sample.Tests;

public class TestFixture : StashboxWebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton(new Mock<ILogger>().Object);
        });
        base.ConfigureWebHost(builder);
    }
}