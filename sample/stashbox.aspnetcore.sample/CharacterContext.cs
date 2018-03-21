using Microsoft.EntityFrameworkCore;
using Stashbox.AspNetCore.Sample.Entity;

namespace Stashbox.AspNetCore.Sample
{
    public class CharacterContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }

        public CharacterContext(DbContextOptions options)
            : base(options)
        { }
    }
}
