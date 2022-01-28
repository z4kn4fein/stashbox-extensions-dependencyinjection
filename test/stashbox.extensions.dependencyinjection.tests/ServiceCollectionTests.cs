using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stashbox.Lifetime;
using System;
using System.Linq;
using Xunit;

namespace Stashbox.Extensions.DependencyInjection.Tests
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void Transient_Configuration_Action()
        {
            var services = new ServiceCollection();
            services.AddTransient(typeof(IService), typeof(Service1), c => c.WithName("s1"));
            services.AddTransient(typeof(IService), typeof(Service2), c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
        }

        [Fact]
        public void Transient_Configuration_Action_Generic()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service1>("s1");
            services.AddTransient<IService, Service2>(c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
        }

        [Fact]
        public void Scoped_Configuration_Action()
        {
            var services = new ServiceCollection();
            services.AddScoped(typeof(IService), typeof(Service1), c => c.WithName("s1"));
            services.AddScoped(typeof(IService), typeof(Service2), c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
        }

        [Fact]
        public void Scoped_Configuration_Action_Generic()
        {
            var services = new ServiceCollection();
            services.AddScoped<IService, Service1>("s1");
            services.AddScoped<IService, Service2>(c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");
            var service3 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
            Assert.IsType<Service2>(service3);

            Assert.Same(service3, service2);
        }

        [Fact]
        public void Singleton_Configuration_Action()
        {
            var services = new ServiceCollection();
            services.AddSingleton(typeof(IService), typeof(Service1), c => c.WithName("s1"));
            services.AddSingleton(typeof(IService), typeof(Service2), c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");
            var service3 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
            Assert.IsType<Service2>(service3);

            Assert.Same(service3, service2);
        }

        [Fact]
        public void Singleton_Configuration_Action_Generic()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IService, Service1>("s1");
            services.AddSingleton<IService, Service2>(c => c.WithName("s2"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>("s1");
            var service2 = resolver.Resolve<IService>("s2");
            var service3 = resolver.Resolve<IService>("s2");

            Assert.IsType<Service1>(service1);
            Assert.IsType<Service2>(service2);
            Assert.IsType<Service2>(service3);

            Assert.Same(service3, service2);
        }

        [Fact]
        public void Decorator()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service1>();
            services.Decorate(typeof(IService), typeof(ServiceDecorator), c => c.WithDependencyBinding("Decorated"));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>();

            Assert.IsType<ServiceDecorator>(service1);
            Assert.IsType<Service1>(((ServiceDecorator)service1).Decorated);
        }

        [Fact]
        public void Decorator_Generic()
        {
            var services = new ServiceCollection();
            services.AddTransient<IService, Service1>();
            services.Decorate<IService, ServiceDecorator>(c => c.WithDependencyBinding(s => s.Decorated));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var service1 = resolver.Resolve<IService>();

            Assert.IsType<ServiceDecorator>(service1);
            Assert.IsType<Service1>(((ServiceDecorator)service1).Decorated);
        }

        [Fact]
        public void Assembly_Scan()
        {
            var services = new ServiceCollection();
            services.ScanAssemblyOf<IService>(
                type => typeof(ServiceBase).IsAssignableFrom(type), // select only the ServiceBase types from the assembly
                (implementationType, serviceType) => serviceType.IsInterface, // register only by interfaces
                false, // do not map services to themselves. E.g: Service -> Service.
                config =>
                {
                    // register IService instances as scoped
                    if (config.ServiceType == typeof(IService))
                        config.WithScopedLifetime();
                }
           );

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var instances = resolver.ResolveAll<IService>();

            Assert.Single(instances);
            Assert.Null(resolver.Resolve<ServiceBase>(true));
            Assert.Null(resolver.Resolve<Service3>(true));
        }

        [Fact]
        public void ComposeBy()
        {
            var services = new ServiceCollection();
            services.ComposeBy(typeof(CompositionRoot));

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var instance = resolver.Resolve<IService>();

            Assert.IsType<Service1>(instance);
        }

        [Fact]
        public void ComposeBy_Generic()
        {
            var services = new ServiceCollection();
            services.ComposeBy<CompositionRoot>();

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var instance = resolver.Resolve<IService>();

            Assert.IsType<Service1>(instance);
        }

        [Fact]
        public void Compose_Assembly()
        {
            var services = new ServiceCollection();
            services.ComposeAssembly(this.GetType().Assembly);

            var serviceProvider = services.UseStashbox();
            var resolver = serviceProvider.GetRequiredService<IDependencyResolver>();

            var instance = resolver.Resolve<IService>();

            Assert.IsType<Service1>(instance);
        }

        [Fact]
        public void RootProviderEquality()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddScoped<IService, Service1>();
            collection.Add(new ServiceDescriptor(typeof(ServiceProviderAware), typeof(ServiceProviderAware), ServiceLifetime.Scoped));
            var provider = collection.UseStashbox();
            var sameProvider = provider.GetRequiredService<IServiceProvider>();
            var sameProvider2 = provider.GetRequiredService<IServiceProvider>();

            // Assert
            Assert.Same(sameProvider, sameProvider2);
            Assert.Same(provider, sameProvider);
        }

        class CompositionRoot : ICompositionRoot
        {
            public void Compose(IStashboxContainer container)
            {
                container.Register<IService, Service1>();
            }
        }

        interface IService { }

        class ServiceBase { }

        class Service1 : IService { }

        class Service2 : IService { }

        class Service3 : ServiceBase, IService { }

        class ServiceDecorator : IService
        {
            public IService Decorated { get; set; }
        }

        class ServiceProviderAware
        {
            public ServiceProviderAware(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }
        }
    }
}
