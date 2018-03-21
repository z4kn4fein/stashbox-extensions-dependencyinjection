using Moq;
using Stashbox.AspNetCore.Sample.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
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
            var testName = "Hasimir";

            var testCharacterResult = new[] { new Character { Name = testName } };
            this.fixture.Stash.Mock<IRepository<Character>>().Setup(r => r.GetAllAsync()).ReturnsAsync(() => testCharacterResult);

            var result = await this.fixture.GetAllAsync();
            Assert.Equal(testName, result.First().Name);

            result = await this.fixture.GetAllAsync();
            Assert.Equal(testName, result.First().Name);

            result = await this.fixture.GetAllAsync();
            Assert.Equal(testName, result.First().Name);

            // wait for the cache to be invalidated
            await Task.Delay(TimeSpan.FromSeconds(5));

            result = await this.fixture.GetAllAsync();
            Assert.Equal(testName, result.First().Name);

            this.fixture.Stash.Mock<IRepository<Character>>().Verify(r => r.GetAllAsync(), Times.Exactly(2));
        }
    }
}
