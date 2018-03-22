# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox-extensions-dependencyinjection/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox-extensions-dependencyinjection)

Stashbox.Extensions.Dependencyinjection: [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Dependencyinjection)](https://www.nuget.org/packages/Stashbox.Extensions.Dependencyinjection/)

Stashbox.AspNetCore.Hosting: [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Hosting)](https://www.nuget.org/packages/Stashbox.AspNetCore.Hosting/)

[Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) integration and [Microsoft.AspNetCore.Hosting](https://github.com/aspnet/Hosting) `IWebHostBuilder` adapter for ASP.NET Core.

## Stashbox.Extensions.Dependencyinjection
Adds an `IServiceProvider` implementation and the `UseStashbox(...)` extension method to the `IServiceCollection` interface, which can be used as the return value of the `ConfigureServices(IServiceCollection services)` method of the `Startup` class.

```c#
public class Startup
{
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IService1, IService1>();
        services.AddTransient<..., ...>();
        
        return services.UseStashbox(container =>
        {
            container.RegisterScoped<IService2, Service2>();
            container.RegisterScoped<..., ...>();
            container.Configure(config => config.WithOptionalAndDefaultValueInjection());
        });
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
With this type of integration the ASP.NET Core runtime will look for a `ConfigureContainer(IStashboxContainer container)` method on the `Startup` class to configure the given container.
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

## Controllers
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
