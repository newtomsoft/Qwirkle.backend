using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Persistence.Models;

namespace Qwirkle.Infra.Persistence
{
    public class DefaultDbContext : IdentityDbContext<UserPersistence, IdentityRole<int>, int>
    {
        public DbSet<TilePersistence> Tiles { get; set; }
        public DbSet<TileOnBagPersistence> TilesOnBag { get; set; }
        public DbSet<TileOnGamePersistence> TilesOnGame { get; set; }
        public DbSet<TileOnPlayerPersistence> TilesOnPlayer { get; set; }
        public DbSet<GamePersistence> Games { get; set; }
        public override DbSet<UserPersistence> Users { get; set; } // todo override à vérifier
        public DbSet<PlayerPersistence> Players { get; set; }


        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
            // Method intentionally left empty.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
