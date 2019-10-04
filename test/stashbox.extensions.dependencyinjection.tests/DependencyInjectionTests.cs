using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stashbox.Attributes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Stashbox.Extensions.DependencyInjection.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public async Task ServiceProviderTest()
        {
            using (var server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>()))
            using (var client = server.CreateClient())
            using (var response = await client.GetAsync("api/test/value"))
            {
                response.EnsureSuccessStatusCode();
                Assert.Equal("test", await response.Content.ReadAsStringAsync());
            }
        }

        [Fact]
        public async Task WebhostBuilderTest()
        {
            using (var server = new TestServer(new WebHostBuilder().UseStashbox().UseStartup<TestStartup2>()))
            using (var client = server.CreateClient())
            using (var response = await client.GetAsync("api/test/value"))
            {
                response.EnsureSuccessStatusCode();
                Assert.Equal("test", await response.Content.ReadAsStringAsync());
            }
        }

        [Fact]
        public async Task ScopedDependencyInjectionTest_WebhostBuilder()
        {
            using (var server = new TestServer(new WebHostBuilder()
                .UseStashbox(container => container.RegisterScoped<TestDependency2>())
                .UseStartup<TestStartup2>()))
            using (var client = server.CreateClient())
            {
                using (var response = await client.GetAsync("api/test2/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("1test1test1", await response.Content.ReadAsStringAsync());
                }

                using (var response = await client.GetAsync("api/test2/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("2test2test2", await response.Content.ReadAsStringAsync());
                }
            }
        }

        [Fact]
        public async Task ScopedDependencyInjectionTest_WebhostBuilder_WithInjectedContainer()
        {
            var container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            container.RegisterScoped<TestDependency2>();
            using (var server = new TestServer(new WebHostBuilder()
                .UseStashbox(container)
                .UseStartup<TestStartup2>()))
            using (var client = server.CreateClient())
            {
                using (var response = await client.GetAsync("api/test2/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("3test3test3", await response.Content.ReadAsStringAsync());
                }

                using (var response = await client.GetAsync("api/test2/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("4test4test4", await response.Content.ReadAsStringAsync());
                }
            }
        }

        [Fact]
        public async Task ScopedDependencyInjectionTest_ServiceProvider()
        {
            using (var server = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup3>()))
            using (var client = server.CreateClient())
            {
                using (var response = await client.GetAsync("api/test3/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("1test1test1", await response.Content.ReadAsStringAsync());
                }

                using (var response = await client.GetAsync("api/test3/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("2test2test2", await response.Content.ReadAsStringAsync());
                }
            }
        }

        [Fact]
        public async Task ScopedDependencyInjectionTest_ServiceProvider_WithInjectedContainer()
        {
            using (var server = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup4>()))
            using (var client = server.CreateClient())
            {
                using (var response = await client.GetAsync("api/test3/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("3test3test3", await response.Content.ReadAsStringAsync());
                }

                using (var response = await client.GetAsync("api/test3/value"))
                {
                    response.EnsureSuccessStatusCode();
                    Assert.Equal("4test4test4", await response.Content.ReadAsStringAsync());
                }
            }
        }
    }

    public class TestStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            return services.UseStashbox(container => container.RegisterScoped<TestDependency>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#if NETCOREAPP3_0
            app.UseRouting().UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
#else
            app.UseMvc();
#endif
        }
    }


    public class TestStartup2
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
        }

        public void ConfigureContainer(IStashboxContainer container)
        {
            container.RegisterScoped<TestDependency>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#if NETCOREAPP3_0
            app.UseRouting().UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
#else
            app.UseMvc();
#endif
        }
    }

    public class TestStartup3
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddScoped<TestDependency3>();
            return services.UseStashbox();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#if NETCOREAPP3_0
            app.UseRouting().UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
#else
            app.UseMvc();
#endif
        }
    }

    public class TestStartup4
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            container.RegisterScoped<TestDependency3>();
            return services.UseStashbox(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#if NETCOREAPP3_0
            app.UseRouting().UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
#else
            app.UseMvc();
#endif
        }
    }

    public class TestDependency
    {
        public string Value => "test";
    }

    [Route("api/test")]
    public class TestController : Controller
    {
        private readonly TestDependency testDependency;

        public TestController(TestDependency testDependency)
        {
            this.testDependency = testDependency;
        }

        [HttpGet("value")]
        public string GetValue()
        {
            return this.testDependency.Value;
        }
    }

    public class TestDependency2
    {
        private static int counter;

        public TestDependency2()
        {
            Interlocked.Increment(ref counter);
        }

        public string Value => "test" + counter;
    }

    [Route("api/test2")]
    public class Test2Controller : Controller
    {
        [Dependency]
        public TestDependency2 TestDependency { get; set; }
        private readonly TestDependency2 testDependency1;

        private static int controllerCounter;

        public Test2Controller(TestDependency2 testDependency1)
        {
            this.testDependency1 = testDependency1;

            Interlocked.Increment(ref controllerCounter);
        }

        [HttpGet("value")]
        public string GetValue()
        {
            return controllerCounter + this.TestDependency.Value + this.testDependency1.Value;
        }
    }

    public class TestDependency3
    {
        private static int counter;

        public TestDependency3()
        {
            Interlocked.Increment(ref counter);
        }

        public string Value => "test" + counter;
    }

    [Route("api/test3")]
    public class Test3Controller : Controller
    {
        private readonly TestDependency3 testDependency;
        private readonly TestDependency3 testDependency1;

        private static int controllerCounter;

        public Test3Controller(TestDependency3 testDependency, TestDependency3 testDependency1)
        {
            this.testDependency = testDependency;
            this.testDependency1 = testDependency1;

            Interlocked.Increment(ref controllerCounter);
        }

        [HttpGet("value")]
        public string GetValue()
        {
            return controllerCounter + this.testDependency.Value + this.testDependency1.Value;
        }
    }
}