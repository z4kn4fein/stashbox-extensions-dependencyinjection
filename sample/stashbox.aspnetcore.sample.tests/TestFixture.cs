using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Stashbox.AspNetCore.Sample.Entity;
using Stashbox.Mocking.Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Sample.Tests
{
    public class TestFixture : WebApplicationFactory<TestStartup>
    {
        public StashMoq Stash { get; }

        public TestFixture()
        {
            this.Stash = StashMoq.Create();
        }

        public async Task<IEnumerable<Character>> GetAllAsync() =>
            JsonConvert.DeserializeObject<IEnumerable<Character>>(await (await this.CreateClient().GetAsync("api/characters")).Content.ReadAsStringAsync());

        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .UseStashbox(this.Stash.Container)
                .ConfigureWebHostDefaults(c => c
                    .UseStartup<TestStartup>()
                    .UseEnvironment("Testing"));
    }
}
