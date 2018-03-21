using Microsoft.EntityFrameworkCore;
using Stashbox.AspNetCore.Sample.Entity;
using System.Threading.Tasks;

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

        public Task AddAsync(Character entity) =>
            this.context.Characters.AddAsync(entity);

        public Task<Character[]> GetAllAsync() =>
            this.context.Characters.ToArrayAsync();

        public Task SaveAsync() =>
            this.context.SaveChangesAsync();
    }
}
