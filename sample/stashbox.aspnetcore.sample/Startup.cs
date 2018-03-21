using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stashbox.AspNetCore.Sample.Entity;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new Info { Title = "Character API", Version = "v1" });
                        options.DescribeAllEnumsAsStrings();
                    })
                    .AddDbContext<CharacterContext>(options => options.UseInMemoryDatabase("character"))
                    .AddMvc()
                    .AddControllersAsServices();
        }

        public virtual void ConfigureContainer(IStashboxContainer container)
        {
            container.RegisterScoped<IRepository<Character>, CharacterRepository>()
                     .RegisterSingleton<ILogger, CustomLogger>();
        }

        public void ConfigureDevelopment(IApplicationBuilder app, IHostingEnvironment env) =>
            app.UseDeveloperExceptionPage()
               .UseTestData()
               .UseGlobalConfiguration();

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
            app.UseGlobalConfiguration();
    }

    public static class AppBuilderStartupExtensions
    {
        public static IApplicationBuilder UseGlobalConfiguration(this IApplicationBuilder app) =>
            app.UseMvc()
               .UseSwagger()
               .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Character API"));

        public static IApplicationBuilder UseTestData(this IApplicationBuilder app)
        {
            var repo = app.ApplicationServices.GetRequiredService<IRepository<Character>>();

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
