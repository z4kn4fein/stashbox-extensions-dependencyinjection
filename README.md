# stashbox-extensions-dependencyinjection
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions-dependencyinjection/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions-dependencyinjection/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox-extensions-dependencyinjection/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox-extensions-dependencyinjection)

Stashbox.Extensions.Dependencyinjection: [![NuGet Version](https://buildstats.info/nuget/Stashbox.Extensions.Dependencyinjection)](https://www.nuget.org/packages/Stashbox.Extensions.Dependencyinjection/)

Stashbox.AspNetCore.Hosting: [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNetCore.Hosting)](https://www.nuget.org/packages/Stashbox.AspNetCore.Hosting/)

[Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) and [Microsoft.AspNetCore.Hosting](https://github.com/aspnet/Hosting) `IWebHostBuilder` adapter for ASP.NET Core.

##Registering in Startup.cs
```c#
public class Startup
{
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        return services.UseStashbox(config =>
        {
            config.RegisterScoped<IService1, Service1>();
            //etc...
        });
    }
}
```
##Registering in the `WebHostBuilder`
```c#
public class Program
{
    public static void Main(string[] args)
    {
        var host = new WebHostBuilder()
        //...
        .UseStashbox(config =>
        {
            config.RegisterScoped<IService1, Service1>();
            //etc...
        })
        //...
        .Build();

        host.Run();
    }
}
```
