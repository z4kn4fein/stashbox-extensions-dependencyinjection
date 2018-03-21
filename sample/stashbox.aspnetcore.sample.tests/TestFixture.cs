using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
using Newtonsoft.Json;
using Stashbox.AspNetCore.Sample.Entity;
using Stashbox.Mocking.Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stashbox.AspNetCore.Sample.Tests
{
    public class TestFixture : IDisposable
    {
        private readonly TestServer server;
        private readonly HttpClient httpClient;

        public StashMoq Stash { get; }

        public TestFixture()
        {
            this.Stash = StashMoq.Create(MockBehavior.Strict);
            this.server = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseEnvironment("Testing")
                .UseStartup<TestStartup>()
                .UseStashbox(this.Stash.Container));

            this.httpClient = this.server.CreateClient();
        }

        public async Task<IEnumerable<Character>> GetAllAsync() =>
            JsonConvert.DeserializeObject<IEnumerable<Character>>(await (await this.httpClient.GetAsync("api/characters")).Content.ReadAsStringAsync());

        public void Dispose()
        {
            this.Stash.Dispose();
            this.httpClient.Dispose();
            this.server.Dispose();
        }
    }
}
