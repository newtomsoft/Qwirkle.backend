using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Repository.Models;

namespace Qwirkle.Infra.Repository
{
    public class DefaultDbContext : IdentityDbContext<UserModel, IdentityRole<int>, int>
    {
        public DbSet<TileModel> Tiles { get; set; }
        public DbSet<TileOnBagModel> TilesOnBag { get; set; }
        public DbSet<TileOnBoardModel> TilesOnBoard { get; set; }
        public DbSet<TileOnPlayerModel> TilesOnPlayer { get; set; }
        public DbSet<GameModel> Games { get; set; }
        public override DbSet<UserModel> Users { get; set; } // todo override à vérifier
        public DbSet<PlayerModel> Players { get; set; }


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
