using Microsoft.EntityFrameworkCore;
using Tenisu.Domain.Entities;

namespace Tenisu.Infrastructure.Context
{
    public sealed class TenisuDbContext : DbContext
    {
        public DbSet<Player> Players {  get; set; }
        public DbSet<Country> Countries {  get; set; }

        public TenisuDbContext(DbContextOptions<TenisuDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenisuDbContext).Assembly);
    }
}
