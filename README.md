# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox-extensions-dependencyinjection/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox-extensions-dependencyinjection) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-extensions-dependencyinjection/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/build/tests) [![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

| Package | Version |
| --- | --- |
| Stashbox.Extensions.Dependencyinjection | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Dependencyinjection)](https://www.nuget.org/packages/Stashbox.Extensions.Dependencyinjection/) |
| Stashbox.AspNetCore.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Hosting)](https://www.nuget.org/packages/Stashbox.AspNetCore.Hosting/) |
| Stashbox.Extensions.Hosting | [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Hosting)](https://www.nuget.org/packages/Stashbox.Extensions.Hosting/) |

This package is an integration for the [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) framework and contains extensions for the [IWebHostBuilder](https://github.com/aspnet/Hosting/blob/master/src/Microsoft.AspNetCore.Hosting.Abstractions/IWebHostBuilder.cs) and [IHostBuilder](https://github.com/aspnet/Hosting/blob/master/src/Microsoft.Extensions.Hosting.Abstractions/IHostBuilder.cs) interfaces.

## ASP.NET Core 3.0
With the changes introduced in ASP.NET Core 3.0 we have the option to use the [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) to host our web application. You can use the *Stashbox.Extension.Hosting* package to integrate Stashbox:
```c#
public static IHostBuilder CreateHostBuilder(String[] args)
{
    return Host.CreateDefaultBuilder(args)
        .UseStashbox()
        .ConfigureWebHostDefaults(
            webBuilder => webBuilder
                .UseStartup<Startup>());
}
```

## Stashbox.Extensions.Dependencyinjection
Contains an `IServiceProvider` implementation which can be used to set [Stashbox](https://github.com/z4kn4fein/stashbox) as the default service provider of the framework. Also contains extension methods (`UseStashbox(...)`) defined on the `IServiceCollection` interface, whichs result you can use as the return value of the `ConfigureServices(IServiceCollection services)` method of your `Startup` class.

You have the option to add your services into the `IServiceCollection` and use `Stashbox` at the end of your configuration section:
```c#
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IService1, Service1>();
        
    return services.UseStashbox();
}
```
Or you can configure your services directly through `Stashbox`:
```c#
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    return services.UseStashbox(container =>
    {
        container.RegisterScoped<IService1, Service1>();
        container.Configure(config => config.WithOptionalAndDefaultValueInjection());
    });
}
```
### Controllers
If you want to let the runtime activate your controllers through Stashbox, you can register them into the service collection:
```c#
public class Startup
{
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddMvc().AddControllersAsServices();
    }
}
```
## Stashbox.AspNetCore.Hosting
Adds the `UseStashbox(...)` extension method to the `IWebHostBuilder`.

```c#
public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseStashbox()
            .Build();
}
```
With this type of integration the ASP.NET Core runtime will optionally look for a `ConfigureContainer(IStashboxContainer container)` method on the `Startup` class to configure the given container.
```c#
public class Startup
{
    public void ConfigureContainer(IStashboxContainer container)
    {
        container.RegisterScoped<IService1, Service1>();
        container.Configure(config => config.WithOptionalAndDefaultValueInjection());
    }
}
```

## Stashbox.Extension.Hosting
Adds the `UseStashbox(...)` extension method to the `IHostBuilder` which integrates `Stashbox` easily with your [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host).

```c#
using (var host = new HostBuilder()
    .UseStashbox()
    .ConfigureContainer<IStashboxContainer>((context, container) =>
    {
        container.Register<Foo>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Service>();
    })
    .Build())
{
    // start and use your host
}
```
