# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?logo=appveyor&logoColor=white)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) 
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/z4kn4fein/stashbox-extensions-dependencyinjection/linux-macOS-CI.yml?logo=GitHub&branch=master)](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection/actions/workflows/linux-macOS-CI.yml) 
[![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-extensions-dependencyinjection/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/build/tests) 
[![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

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

### Table of Contents
* [ASP.NET Core](#aspnet-core)
    - [ASP.NET Core 5](#aspnet-core-5)
    - [ASP.NET Core 6](#aspnet-core-6)
  + [Controller / View activation](#controller--view-activation)
    - [ASP.NET Core 5](#aspnet-core-5-1)
    - [ASP.NET Core 6](#aspnet-core-6-1)
  + [Multitenant](#multitenant)
    - [ASP.NET Core 5](#aspnet-core-5-2)
    - [ASP.NET Core 6](#aspnet-core-6-2)
* [.NET Generic Host](#net-generic-host)
* [ServiceCollection Based Applications](#servicecollection-based-applications)
* [Additional IServiceCollection Extensions](#additional-iservicecollection-extensions)

## ASP.NET Core
The following example shows how you can integrate Stashbox (with the `Stashbox.Extensions.Hosting` package) as the default `IServiceProvider` implementation into your ASP.NET Core application:
#### ASP.NET Core 5
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
            .UseStashbox(container => // Optional configuration options.
            {
                // This one enables the lifetime validation for production environments too.
                container.Configure(config => config.WithLifetimeValidation());
            })
            .ConfigureContainer<IStashboxContainer>((context, container) =>
            {
                // Execute a dependency tree validation.
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
        // Your service configuration.
    }

    public void ConfigureContainer(IStashboxContainer container)
    {
        // Your container configuration.
        container.Configure(config => config.WithLifetimeValidation());
    }

    public void Configure(IApplicationBuilder app)
    {
        // Your application configuration.
    }
}
```

#### ASP.NET Core 6
```c#
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseStashbox(container => // Optional configuration options.
{
    // This one enables the lifetime validation for production environments too.
    container.Configure(config => config.WithLifetimeValidation());
});

builder.Host.ConfigureContainer<IStashboxContainer>((context, container) =>
{
    // Execute a dependency tree validation.
    if (context.HostingEnvironment.IsDevelopment())
        container.Validate();
});
```


### Controller / View activation
By default the ASP.NET Core framework uses the `DefaultControllerActivator` to instantiate controllers, but it uses the `ServiceProvider` only for instantiating their constructor dependencies. This behaviour could hide important errors Stashbox would throw in case of a misconfiguration, so it's recommended to let Stashbox activate your controllers and views.  

You can enable this by adding the following options to your service configuration:
#### ASP.NET Core 5
```c#
public void ConfigureServices(IServiceCollection services)
{
    // For controllers only.
    services.AddControllers()
            .AddControllersAsServices();
    
    // For controllers and views.
    services.AddControllersWithViews()
            .AddControllersAsServices()
            .AddViewComponentsAsServices();
}
```
#### ASP.NET Core 6

```c#
// For controllers only.
builder.Services.AddControllers()
    .AddControllersAsServices();
    
// For controllers and views.
builder.Services.AddControllersWithViews()
    .AddControllersAsServices()
    .AddViewComponentsAsServices();
```

### Multitenant
The `Stashbox.AspNetCore.Multitenant` package provides support for multitenant applications with a component called `TenantDistributor`. It's responsible for the following tasks:
1. **Create / maintain the application level Root Container.** This container is used to hold the default service registrations for your application.
1. **Configure / maintain tenant specific containers.** These containers are used to override the default services with tenant specific registrations.
1. **Tenant identification.** Determines the tenant Id based on the current context. To achieve that, you have to provide an `ITenantIdExtractor` implementation.

```c#
// The type used to extract the current tenant identifier.
// This implementation shows how to extract the tenant id from a HTTP header.

public class HttpHeaderTenantIdExtractor : ITenantIdExtractor
{
    public Task<object> GetTenantIdAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("TENANT-ID", out var value))
            return Task.FromResult<object>(null);

        return Task.FromResult<object>(value.First());
    }
}
```
#### ASP.NET Core 5
```c#
public static IHostBuilder CreateHostBuilder(String[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseStashboxMultitenant<HttpHeaderTenantIdExtractor>(
            distributor => // The tenant distributor configuration options.
        {
            // The default service registration.
            // It also could be registered into the default 
            // service collection through the ConfigureServices() api.
            distributor.RootContainer.Register<IDependency, DefaultDependency>();

            // Configure tenants.
            distributor.ConfigureTenant("TenantA", container => 
                // Register tenant specific service override
                container.Register<IDependency, TenantASpecificDependency>());

            distributor.ConfigureTenant("TenantB", container => 
                // Register tenant specific service override
                container.Register<IDependency, TenantBSpecificDependency>());
        })
        .ConfigureContainer<TenantDistributor>((context, distributor) =>
        {
            // Validate the root container and all the tenants.
            if (context.HostingEnvironment.IsDevelopment())
                distributor.Validate();
        })
        .ConfigureWebHostDefaults(
            webBuilder => webBuilder
                .UseStartup<Startup>());
    }
```
#### ASP.NET Core 6
```c#
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseStashboxMultitenant<HttpHeaderTenantIdExtractor>(
    distributor => // The tenant distributor configuration options.
{
    // The default service registration.
    // It also could be registered into the default 
    // service collection through the ConfigureServices() api.
    distributor.RootContainer.Register<IDependency, DefaultDependency>();

    // Configure tenants.
    distributor.ConfigureTenant("TenantA", container => 
        // Register tenant specific service override
        container.Register<IDependency, TenantASpecificDependency>());

    distributor.ConfigureTenant("TenantB", container => 
        // Register tenant specific service override
        container.Register<IDependency, TenantBSpecificDependency>());
});

builder.Host.ConfigureContainer<TenantDistributor>((context, distributor) =>
{
    // Validate the root container and all the tenants.
    if (context.HostingEnvironment.IsDevelopment())
        distributor.Validate();
});
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
            .UseStashbox(container => // Optional configuration options.
            {
                // This one enables the lifetime validation for production environments too.
                container.Configure(config => config.WithLifetimeValidation());
            })
            .ConfigureContainer<IStashboxContainer>((context, container) =>
            {
                // Execute a dependency tree validation.
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

## ServiceCollection Based Applications
With the `Stashbox.Extensions.Dependencyinjection` package you can replace Microsoft's built-in dependency injection container with Stashbox. This package contains the core functionality used by the `Stashbox.Extensions.Hosting`, `Stashbox.AspNetCore.Hosting` and `Stashbox.AspNetCore.Multitenant` packages.

The following example shows how you can use this integration:
```c#
public class Program
{
    public static async Task Main(string[] args)
    {
        // Create the service collection.
        var services = new ServiceCollection();

        // Configure your service collection.
        services.AddLogging();
        services.AddOptions();

        // Add your services.
        services.AddScoped<IService, Service>();

        // Integrate Stashbox with the collection and grab your ServiceProvider.
        var serviceProvider = services.UseStashbox(container => // Optional configuration options.
        {
            container.Configure(config => config.WithLifetimeValidation());
        });

        // Start using the application.
        using (var scope = serviceProvider.CreateScope())
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
        // Create your container.
        var container = new StashboxContainer(config => // Optional configuration options.
        {
            config.WithLifetimeValidation();
        });

        // Create the service collection.
        var services = new ServiceCollection();

        // Configure your service collection.
        services.AddLogging();
        services.AddOptions();

        // Add your services.
        services.AddScoped<IService, Service>();

        // Or add them through Stashbox.
        container.RegisterScoped<IService, Service>();

        // Integrate Stashbox with the collection.
        services.UseStashbox(container);

        // Execute a dependency tree validation.
        container.Validate();

        // Start using the application.
        await using (var scope = container.BeginScope())
        {
            var service = scope.Resolve<IService>();
            await service.DoSomethingAsync();
        }
    }
}
```

## Additional `IServiceCollection` Extensions
Most of Stashbox's service registration functionalities are available as extension methods of `IServiceCollection`.

- [Named service registration](https://z4kn4fein.github.io/stashbox/#/usage/basics?id=named-registration):
  ```csharp
  class Service2 : IService2
  {
      private readonly IService service;

      public Service2(IService service) 
      {
          this.service = service;
      }
  }
  
  var services = new ServiceCollection();
  services.AddTransient<IService, Service>(); // Name-less registration.
  services.AddTransient<IService, AnotherService>("serviceName"); // Register dependency with name.
  services.AddTransient<IService2, Service2>(config => 
    // Inject the named service as dependency.
    config.WithDependencyBinding<IService>(
        "serviceName" // Name of the dependency.
    ));
  ```

- Service configuration with Stashbox's [Fluent Registration API](https://z4kn4fein.github.io/stashbox/#/configuration/registration-configuration?id=registration-configuration):
  ```csharp
  var services = new ServiceCollection();
  services.AddTransient<IService, Service>(config => 
    config.WithFactory<IDependency>(dependency => new Service(dependency)).AsImplementedTypes());
  ```

- [Service decoration](https://z4kn4fein.github.io/stashbox/#/advanced/decorators):
  ```csharp
  class ServiceDecorator : IService
  {
      private readonly IService decorated;

      public ServiceDecorator(IService service)
      {
          this.decorated = service;
      }
  }

  var services = new ServiceCollection();
  services.AddTransient<IService, Service>();
  services.Decorate<IService, ServiceDecorator>();
  ```

- [Assembly registration](https://z4kn4fein.github.io/stashbox/#/usage/advanced-registration?id=assembly-registration):
  ```csharp
  var services = new ServiceCollection();
  services.ScanAssemblyOf<IService>(
    // Set a filter for which types should be excluded/included in the registration process.
    // In this case, only the publicly available types are selected from the assembly.
    type => type.IsPublic, 
    // The service type selector. Used to filter which interface or base types the implementation should be mapped to.
    // In this case, we are registering only by interfaces.
    (implementationType, serviceType) => serviceType.IsInterface,
    false, // Do not map services to themselves. E.g: Service -> Service.
    config =>
    {
        // Register IService instances as scoped.
        if (config.ServiceType == typeof(IService))
            config.WithScopedLifetime();
    }
  );
  ```

- [Composition root](https://z4kn4fein.github.io/stashbox/#/usage/advanced-registration?id=composition-root):
  ```csharp
  class CompositionRoot : ICompositionRoot
  {
      public void Compose(IStashboxContainer container)
      {
          container.Register<IService, Service>();
      }
  }

  var services = new ServiceCollection();
  services.ComposeBy<CompositionRoot>();
  
  // Or let Stashbox find all composition roots in an assembly.
  services.ComposeAssembly(typeof(CompositionRoot).Assembly);
  ```
