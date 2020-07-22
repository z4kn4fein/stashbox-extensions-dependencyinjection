# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox-extensions-dependencyinjection/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox-extensions-dependencyinjection) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-extensions-dependencyinjection/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/build/tests) [![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

This repository contains integrations for [ASP.NET Core](#aspnet-core), [.NET Generic Host](#net-generic-host) and simple [ServiceCollection](#servicecollection-based-applications) based applications.

| Package | Version |
| --- | --- |
| Stashbox.Extensions.Dependencyinjection | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Dependencyinjection)](https://www.nuget.org/packages/Stashbox.Extensions.Dependencyinjection/) |
| Stashbox.Extensions.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Hosting)](https://www.nuget.org/packages/Stashbox.Extensions.Hosting/) |
| Stashbox.AspNetCore.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Hosting)](https://www.nuget.org/packages/Stashbox.AspNetCore.Hosting/) |

### Options turned on by default:
- Automatic tracking and disposal of `IDisposable` and `IAsyncDisposable` services.
- Lifetime validation for `Developement` environments, but can be extended to all environment types.

## ASP.NET Core
The following example shows how you can integrate Stashbox (with the `Stashbox.AspNetCore.Hosting` package) as the default `IServiceProvider` implementation into your ASP.NET Core application:
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
                // but you can use the configuration callback provided by the framework.
                container.Configure(config => config.WithAutoMemberInjection());
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

## .NET Generic Host
The following example adds Stashbox (with the `Stashbox.AspNetCore.Hosting` package) as the default `IServiceProvider` implementation into your [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1) application:

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
                // but you can use the configuration callback provided by the framework.
                container.Configure(config => config.WithAutoMemberInjection());
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
With the `Stashbox.Extensions.Dependencyinjection` package you can replace Microsoft's built-in dependency injection container with Stashbox. This package contains the core functionality used by the `Stashbox.Extensions.Hosting` and `Stashbox.AspNetCore.Hosting` packages.

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

        // start using the application.
        await using (var scope = container.BeginScope())
        {
            var service = scope.Resolve<IService>();
            await service.DoSomethingAsync();
        }
    }
}
```
