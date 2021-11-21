using Stashbox.AspNetCore.Sample;
using Stashbox.AspNetCore.Sample.Entity;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> UseTestDataAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<Character>>();

            IEnumerable<Task> ReadTestData()
            {
                foreach (var character in app.ApplicationServices.GetRequiredService<IConfiguration>().GetSection("TestData").Get<List<Character>>())
                    yield return repo.AddAsync(character);
            }

            await Task.WhenAll(ReadTestData());
            await repo.SaveAsync();
            return app;
        }
    }
}
