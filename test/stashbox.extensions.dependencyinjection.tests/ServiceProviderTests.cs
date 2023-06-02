using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Stashbox.Exceptions;
using Xunit;

namespace Stashbox.Extensions.DependencyInjection.Tests;

public class ServiceProviderTests
{
    [Fact]
    public void Named_GetService()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(IService), typeof(Service1), c => c.WithName("s1"));
        services.AddTransient(typeof(IService), typeof(Service2), c => c.WithName("s2"));

        var serviceProvider = services.UseStashbox();

        var service1 = serviceProvider.GetService<IService>("s1");
        var service2 = serviceProvider.GetService(typeof(IService), "s2");

        Assert.IsType<Service1>(service1);
        Assert.IsType<Service2>(service2);
    }
    
    [Fact]
    public void Named_GetRequiredService()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(IService), typeof(Service1), c => c.WithName("s1"));
        services.AddTransient(typeof(IService), typeof(Service2), c => c.WithName("s2"));

        var serviceProvider = services.UseStashbox();

        var service1 = serviceProvider.GetRequiredService<IService>("s1");
        var service2 = serviceProvider.GetService(typeof(IService), "s2");

        Assert.IsType<Service1>(service1);
        Assert.IsType<Service2>(service2);
    }
    
    [Fact]
    public void Named_GetServices()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(IService), typeof(Service1), c => c.WithName("s1"));
        services.AddTransient(typeof(IService), typeof(Service2), c => c.WithName("s2"));

        var serviceProvider = services.UseStashbox();

        var service1 = serviceProvider.GetServices<IService>("s1");
        var service2 = serviceProvider.GetServices(typeof(IService), "s2");

        Assert.IsType<Service1>(service1.First());
        Assert.IsType<Service2>(service2.First());
    }
    
    [Fact]
    public void Named_NotSupported_Tests()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(IService), typeof(Service1));
        services.AddTransient(typeof(IService), typeof(Service2));

        var serviceProvider = services.BuildServiceProvider();

        Assert.Throws<NotSupportedException>(() => serviceProvider.GetService<IService>("s1"));
        Assert.Throws<NotSupportedException>(() => serviceProvider.GetService(typeof(IService), "s2"));
        
        Assert.Throws<NotSupportedException>(() => serviceProvider.GetRequiredService<IService>("s1"));
        Assert.Throws<NotSupportedException>(() => serviceProvider.GetRequiredService(typeof(IService), "s2"));
        
        Assert.Throws<NotSupportedException>(() => serviceProvider.GetServices<IService>("s1"));
        Assert.Throws<NotSupportedException>(() => serviceProvider.GetServices(typeof(IService), "s2"));
    }
    
    [Fact]
    public void ServiceProvider_Resolution_Tests()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(SpAware));

        var serviceProvider = services.UseStashbox();
        
        Assert.IsType<StashboxServiceProvider>(serviceProvider.GetRequiredService<SpAware>().ServiceProvider);
#if HAS_IS_SERVICE
        Assert.IsType<StashboxServiceProvider>(serviceProvider.GetRequiredService<SpAware>().ServiceProviderIsService);
#endif
        Assert.IsType<StashboxServiceProvider>(serviceProvider.GetRequiredService<IServiceProvider>());
    }
    
    [Fact]
    public void Named_Result_Tests()
    {
        var services = new ServiceCollection();
        var serviceProvider = services.UseStashbox();
        
        Assert.Null(serviceProvider.GetService(typeof(Service1)));
        Assert.Null(serviceProvider.GetService<Service1>());
        
        Assert.Throws<ResolutionFailedException>(() => serviceProvider.GetRequiredService(typeof(Service1)));
        Assert.Throws<ResolutionFailedException>(() => serviceProvider.GetRequiredService<Service1>());
    }
    
    interface IService { }

    class Service1 : IService { }

    class Service2 : IService { }

    class SpAware
    {
#if HAS_IS_SERVICE
        public IServiceProviderIsService ServiceProviderIsService { get; }
#endif
        public IServiceProvider ServiceProvider { get; }

        public SpAware(IServiceProvider serviceProvider
#if HAS_IS_SERVICE
            , IServiceProviderIsService serviceProviderIsService
#endif
            )
        {
#if HAS_IS_SERVICE
            ServiceProviderIsService = serviceProviderIsService;
#endif
            ServiceProvider = serviceProvider;
        }
    }
}