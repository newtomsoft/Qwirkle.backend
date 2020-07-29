using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Persistance.Models;

namespace Qwirkle.Infra.Persistance
{
    public class DefaultDbContext : IdentityDbContext<PlayerPersistance, IdentityRole<int>, int>
    {
        public DbSet<TileOnBagPersistance> TilesOnBag { get; set; }
        public DbSet<TileOnBoardPersistance> TilesOnBoard { get; set; }
        public DbSet<GamePersistance> Games { get; set; }
        public DbSet<PlayerPersistance> Players { get; set; }
        public DbSet<GamePlayerPersistance> GamePlayers { get; set; }



        public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Method intentionally left empty.
        }
    }
}
