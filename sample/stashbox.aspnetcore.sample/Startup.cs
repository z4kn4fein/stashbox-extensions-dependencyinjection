using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Stashbox.AspNetCore.Sample.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(typeof(Startup).Assembly);
            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Character API", Version = "v1" });
                })
                .AddDbContext<CharacterContext>(options => options.UseInMemoryDatabase("character"))
                .AddMemoryCache();
        }

        public virtual void ConfigureContainer(IStashboxContainer container) =>
            container.RegisterScoped<IRepository<Character>, CharacterRepository>()
                     .RegisterSingleton<ILogger, CustomLogger>();

        public void ConfigureDevelopment(IApplicationBuilder app) =>
            app.UseDeveloperExceptionPage()
               .UseTestData()
               .UseGlobalConfiguration();

        public void Configure(IApplicationBuilder app) =>
            app.UseGlobalConfiguration();
    }

    public static class AppBuilderStartupExtensions
    {
        public static IApplicationBuilder UseGlobalConfiguration(this IApplicationBuilder app) =>
            app.UseSwagger()
               .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Character API"))
               .UseRouting()
               .UseEndpoints(endpoints =>
               {
                   endpoints.MapControllers();
               });

        public static IApplicationBuilder UseTestData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Character>>();

            IEnumerable<Task> ReadTestData()
            {
                foreach (var character in app.ApplicationServices.GetRequiredService<IConfiguration>().GetSection("TestData").Get<List<Character>>())
                    yield return repo.AddAsync(character);
            }

            Task.WaitAll(ReadTestData().ToArray());
            repo.SaveAsync().Wait();

            return app;
        }
    }
}
