using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Persistance.Models;

namespace Qwirkle.Infra.Persistance
{
    public class DefaultDbContext : IdentityDbContext<UserPersistance, IdentityRole<int>, int>
    {
        public DbSet<TilePersistance> Tiles { get; set; }
        public DbSet<TileOnBagPersistance> TilesOnBag { get; set; }
        public DbSet<TileOnGamePersistance> TilesOnGame { get; set; }
        public DbSet<TileOnPlayerPersistance> TilesOnPlayer { get; set; }
        public DbSet<GamePersistance> Games { get; set; }
        public override DbSet<UserPersistance> Users { get; set; } // todo override à vérifier
        public DbSet<PlayerPersistance> Players { get; set; }


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
