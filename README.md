# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox-extensions-dependencyinjection/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox-extensions-dependencyinjection) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-extensions-dependencyinjection/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/build/tests) [![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

This repository contains [Stashbox](https://github.com/z4kn4fein/stashbox) integrations for [ASP.NET Core](#aspnet-core), [.NET Generic Host](#net-generic-host) and simple [ServiceCollection](#servicecollection-based-applications) based applications.

| Package | Version |
| --- | --- |
| Stashbox.Extensions.Dependencyinjection | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Dependencyinjection)](https://www.nuget.org/packages/Stashbox.Extensions.Dependencyinjection/) |
| Stashbox.Extensions.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Hosting)](https://www.nuget.org/packages/Stashbox.Extensions.Hosting/) |
| Stashbox.AspNetCore.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Hosting)](https://www.nuget.org/packages/Stashbox.AspNetCore.Hosting/) |
| Stashbox.AspNetCore.Multitenant | [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Multitenant)](https://www.nuget.org/packages/Stashbox.AspNetCore.Multitenant/) |

### Options turned on by default:
- Automatic tracking and disposal of `IDisposable` and `IAsyncDisposable` services.
- Lifetime validation for `Developement` environments, but can be extended to all environment types.

## ASP.NET Core
The following example shows how you can integrate Stashbox (with the `Stashbox.Extensions.Hosting` package) as the default `IServiceProvider` implementation into your ASP.NET Core application:
```c#
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(String[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseStashbox(container => // optional configuration options.
            {
                // this one enables the lifetime validation for production environments too.
                container.Configure(config => config.WithLifetimeValidation());
            })
            .ConfigureContainer<IStashboxContainer>((context, container) =>
            {
                // execute a dependency tree validation.
                if (context.HostingEnvironment.IsDevelopment())
                    container.Validate();
            })
            .ConfigureWebHostDefaults(
                webBuilder => webBuilder
                    .UseStartup<Startup>());
    }
}
```

You can also use the `ConfigureContainer()` method in your `Startup` class to use further configuration options:
```c#

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // your service configuration.
    }

    public void ConfigureContainer(IStashboxContainer container)
    {
        // your container configuration.
        container.Configure(config => config.WithLifetimeValidation());
    }

    public void Configure(IApplicationBuilder app)
    {
        // your application configuration.
    }
}
```

### Controller / View activation
By default the ASP.NET Core framework uses the `DefaultControllerActivator` to instantiate controllers, but it uses the `ServiceProvider` only for instantiating their constructor dependencies. This behaviour could hide important errors Stashbox would throw in case of a misconfiguration, so it's recommended to let Stashbox activate your controllers and views.  

You can enable this by adding the following options to your service configuration:
```c#
public void ConfigureServices(IServiceCollection services)
{
    // for controllers only.
    services.AddControllers()
            .AddControllersAsServices();
    
    // for controllers and views.
    services.AddControllersWithViews()
            .AddControllersAsServices()
            .AddViewComponentsAsServices()
}
```

### Multitenant
The `Stashbox.AspNetCore.Multitenant` package provides support for multitenant applications with a component called `TenantDistributor`. It's responsible for the following tasks:
1. **Create / maintain the application level Root Container.** This container is used to hold the default service registrations for your application.
1. **Configure / maintain tenant specific containers.** These containers are used to override the default services with tenant specific registrations.
1. **Tenant identification.** Determines the tenant Id based on the current context. To achieve that, you have to provide an `ITenantIdExtractor` implementation.

```c#
// the type used to extract the current tenant identifier.
// this implementation shows how to extract the tenant id from a HTTP header.

public class HttpHeaderTenantIdExtractor : ITenantIdExtractor
{
    public Task<object> GetTenantIdAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("TENANT-ID", out var value))
            return Task.FromResult<object>(null);

        return Task.FromResult<object>(value.First());
    }
}

public static IHostBuilder CreateHostBuilder(String[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseStashboxMultitenant<HttpHeaderTenantIdExtractor>(
            distributor => // the tenant distributor configuration options.
        {
            // the default service registration.
            // it also could be registered into the default 
            // service collection through the ConfigureServices() api.
            distributor.RootContainer.Register<IDependency, DefaultDependency>();

            // configure tenants.
            distributor.ConfigureTenant("TenantA", container => 
                // register tenant specific service override
                container.Register<IDependency, TenantASpecificDependency>());

            distributor.ConfigureTenant("TenantB", container => 
                // register tenant specific service override
                container.Register<IDependency, TenantBSpecificDependency>());
        })
        .ConfigureContainer<TenantDistributor>((context, distributor) =>
        {
            // validate the root container and all the tenants.
            if (context.HostingEnvironment.IsDevelopment())
                distributor.Validate();
        })
        .ConfigureWebHostDefaults(
            webBuilder => webBuilder
                .UseStartup<Startup>());
    }
```
With this example setup, you can differentiate tenants in a per-request basis identified by a HTTP header, where every tenant gets their overridden services. 


## .NET Generic Host
The following example adds Stashbox (with the `Stashbox.Extensions.Hosting` package) as the default `IServiceProvider` implementation into your [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1) application:

```c#
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseStashbox(container => // optional configuration options.
            {
                // this one enables the lifetime validation for production environments too.
                container.Configure(config => config.WithLifetimeValidation());
            })
            .ConfigureContainer<IStashboxContainer>((context, container) =>
            {
                // execute a dependency tree validation.
                if (context.HostingEnvironment.IsDevelopment())
                    container.Validate();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Service>();
            }).Build();

        await host.RunAsync();
    }
}
```

## ServiceCollection based applications
With the `Stashbox.Extensions.Dependencyinjection` package you can replace Microsoft's built-in dependency injection container with Stashbox. This package contains the core functionality used by the `Stashbox.Extensions.Hosting`, `Stashbox.AspNetCore.Hosting` and `Stashbox.AspNetCore.Multitenant` packages.

The following example shows how you can use this integration:
```c#
public class Program
{
    public static async Task Main(string[] args)
    {
        // create the service collection.
        var services = new ServiceCollection();

        // configure your service collection.
        services.AddLogging();
        services.AddOptions();

        // add your services.
        services.AddScoped<IService, Service>();

        // integrate Stashbox with the collection and grab your ServiceProvider.
        var serviceProvider = services.UseStashbox(container => // optional configuration options.
        {
            container.Configure(config => config.WithLifetimeValidation());
        });

        // start using the application.
        await using (var scope = serviceProvider.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<IService>();
            await service.DoSomethingAsync();
        }
    }
}
```

Or you can use your own `StashboxContainer` to integrate with the `ServiceCollection`:
```c#
public class Program
{
    public static async Task Main(string[] args)
    {
        // create your container.
        var container = new StashboxContainer(config => // optional configuration options.
        {
            config.WithLifetimeValidation();
        });

        // create the service collection.
        var services = new ServiceCollection();

        // configure your service collection.
        services.AddLogging();
        services.AddOptions();

        // add your services.
        services.AddScoped<IService, Service>();

        // or add them through Stashbox.
        container.RegisterScoped<IService, Service>();

        // integrate Stashbox with the collection.
        services.UseStashbox(container);

        // execute a dependency tree validation.
        container.Validate();

        // start using the application.
        await using (var scope = container.BeginScope())
        {
            var service = scope.Resolve<IService>();
            await service.DoSomethingAsync();
        }
    }
}
```
