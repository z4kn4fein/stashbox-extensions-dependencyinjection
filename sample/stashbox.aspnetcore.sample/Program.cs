using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stashbox;
using Stashbox.AspNetCore.Sample;
using Stashbox.AspNetCore.Sample.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseStashbox();

builder.Host.ConfigureContainer<IStashboxContainer>((context, container) =>
{
    // execute a dependency tree validation.
    if (context.HostingEnvironment.IsDevelopment())
        container.Validate();
});

ConfigureServices(builder.Services);

var app = builder.Build();

ConfigureApplication(app);

if (app.Environment.IsDevelopment())
{
    await app.UseTestDataAsync();
}

await app.RunAsync();

public partial class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddApplicationPart(typeof(Program).Assembly);

        services.AddScoped<IRepository<Character>, CharacterRepository>();
        services.AddSingleton<Stashbox.AspNetCore.Sample.ILogger, CustomLogger>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Character API", Version = "v1" });
        });

        services.AddDbContext<CharacterContext>(options => options.UseInMemoryDatabase("character"))
            .AddMemoryCache();
    }

    public static void ConfigureApplication(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Character API"));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(options =>
        {
            options.MapControllers();
        });
    }
}