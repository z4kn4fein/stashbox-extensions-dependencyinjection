using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Stashbox.AspNetCore.Sample.Entity;

namespace Stashbox.AspNetCore.Sample.Controllers
{
    [Route("api/[controller]")]
    public class CharactersController : Controller
    {
        private const string MemCacheKey = "character";

        private readonly IRepository<Character> characterRepository;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger logger;

        public CharactersController(IRepository<Character> characterRepository, IMemoryCache memoryCache, ILogger logger)
        {
            this.characterRepository = characterRepository;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Character>> Get()
        {
            var result = await this.memoryCache.GetOrCreateAsync(MemCacheKey, async entry =>
            {
                this.logger.Log("Updating cache from DB.");
                entry.SlidingExpiration = TimeSpan.FromSeconds(5);
                return await this.characterRepository.GetAllAsync();
            });
            return result;
        }
            
    }
}
