using Microsoft.EntityFrameworkCore;
using Stashbox.AspNetCore.Sample.Entity;

namespace Stashbox.AspNetCore.Sample
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);

        Task<T[]> GetAllAsync();

        Task SaveAsync();
    }

    public class CharacterRepository : IRepository<Character>
    {
        private readonly CharacterContext context;

        public CharacterRepository(CharacterContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Character entity) =>
            await this.context.Characters.AddAsync(entity);

        public Task<Character[]> GetAllAsync() =>
            this.context.Characters.ToArrayAsync();

        public Task SaveAsync() =>
            this.context.SaveChangesAsync();
    }
}
