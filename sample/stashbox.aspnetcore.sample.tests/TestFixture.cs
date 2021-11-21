using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Stashbox.AspNetCore.Sample.Entity;
using Stashbox.Mocking.Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Sample.Tests
{
    public class TestFixture : WebApplicationFactory<Program>
    {
        public StashMoq Stash { get; }

        private readonly HttpClient client;

        public TestFixture()
        {
            this.Stash = StashMoq.Create();
            this.client = this.CreateClient();
        }

        public async Task<IEnumerable<Character>> GetAllAsync() =>
            JsonConvert.DeserializeObject<IEnumerable<Character>>(await (await this.client.GetAsync("api/characters")).Content.ReadAsStringAsync());

        protected override IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .UseStashbox(this.Stash.Container)
                    .ConfigureWebHost(h => h
                        .ConfigureServices(Program.ConfigureServices)
                        .Configure(app => Program.ConfigureApplication(app)));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.Stash.Dispose();
            
            base.Dispose(disposing);
        }
    }
}
