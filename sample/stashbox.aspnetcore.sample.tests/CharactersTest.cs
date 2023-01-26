using Stashbox.AspNetCore.Sample.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Stashbox.AspNetCore.Sample.Tests
{
    public class CharactersTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture fixture;

        public CharactersTest(TestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task GetAllTest_Ignore_Logger_Completely()
        {
            const string testName = "Hasimir";

            var testCharacterResult = new[] { new Character { Name = testName } };

            var mockRepository = new Mock<IRepository<Character>>();
            mockRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(() => testCharacterResult);

            var client = this.fixture.StashClient((services, _) => { services.AddSingleton(mockRepository.Object); });

            var result = await GetAllCharactersAsync(client);
            Assert.Equal(testName, result?.First().Name);

            result = await GetAllCharactersAsync(client);
            Assert.Equal(testName, result?.First().Name);

            result = await GetAllCharactersAsync(client);
            Assert.Equal(testName, result?.First().Name);

            // wait for the cache to be invalidated
            await Task.Delay(TimeSpan.FromSeconds(5));

            result = await GetAllCharactersAsync(client);
            Assert.Equal(testName, result?.First().Name);

            mockRepository.Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }

        private static async Task<IEnumerable<Character>?> GetAllCharactersAsync(HttpClient client)
        {
            var response = await client.GetAsync("api/Characters");
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<IEnumerable<Character>>(contentStream,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}