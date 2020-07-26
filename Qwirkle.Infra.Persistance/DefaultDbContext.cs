using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Persistance.Models;

namespace Qwirkle.Infra.Persistance
{
    public class DefaultDbContext : DbContext // : IdentityDbContext<Player, IdentityRole<int>, int>  // TODO quand Player implémenté
    {
        public DbSet<TileOnBag> TilesOnBag { get; set; }
        public DbSet<TileOnBoard> TilesOnBoard { get; set; }
        public DbSet<Game> Boards { get; set; }



        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
