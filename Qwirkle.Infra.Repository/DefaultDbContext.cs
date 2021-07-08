using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qwirkle.Infra.Repository.Dao;

namespace Qwirkle.Infra.Repository
{
    public class DefaultDbContext : IdentityDbContext<UserDao, IdentityRole<int>, int>
    {
        public DbSet<TileDao> Tiles { get; set; }
        public DbSet<TileOnBagDao> TilesOnBag { get; set; }
        public DbSet<TileOnBoardDao> TilesOnBoard { get; set; }
        public DbSet<TileOnPlayerDao> TilesOnPlayer { get; set; }
        public DbSet<GameDao> Games { get; set; }
        public override DbSet<UserDao> Users { get; set; } // todo override à vérifier
        public DbSet<PlayerDao> Players { get; set; }


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
