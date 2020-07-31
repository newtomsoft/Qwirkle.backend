using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.Services;
using Qwirkle.Infra.Persistance;
using Qwirkle.Infra.Persistance.Adapters;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Qwirkle.Core.ComplianceContext.Tests
{
    public class InitGameShould
    {
        private ComplianceService ComplianceService { get; set; }
        private ICompliancePersistance Persistance { get; set; }
        private DefaultDbContext DbContext { get; set; }

        private const int GAME_ID = 7;
        private const int USER1 = 71;
        private const int USER2 = 21;
        private const int USER3 = 3;
        private const int USER4 = 14;
        private const int PLAYER9 = 9;
        private const int PLAYER3 = 3;
        private const int PLAYER8 = 8;
        private const int PLAYER14 = 14;
        private const int TOTAL_TILES = 108;

        public InitGameShould()
        {
            Context();
            Persistance = new CompliancePersistanceAdapter(DbContext);
            ComplianceService = new ComplianceService(Persistance);
            AddAllTiles();
            AddUsers();
            AddPlayers();
            AddTilesOnBag();
        }

        private void Context()
        {
            var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContext = new DefaultDbContext(contextOptions);
        }

        private void AddAllTiles()
        {
            const int NUMBER_OF_SAME_TILE = 3;
            int id = 0;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        DbContext.Tiles.Add(new TilePersistance { Id = ++id, Color = color, Form = form });

            DbContext.SaveChanges();
        }

        private void AddTilesOnBag()
        {
            for (int i = 1; i <= TOTAL_TILES; i++)
            {
                DbContext.TilesOnBag.Add(new TileOnBagPersistance { Id = 100 + i, GameId = GAME_ID, TileId = i });
            }
            DbContext.SaveChanges();
        }

        private void AddUsers()
        {
            DbContext.Users.Add(new UserPersistance { Id = USER1 });
            DbContext.Users.Add(new UserPersistance { Id = USER2 });
            DbContext.Users.Add(new UserPersistance { Id = USER3 });
            DbContext.Users.Add(new UserPersistance { Id = USER4 });
            DbContext.SaveChanges();
        }

        private void AddPlayers()
        {
            DbContext.Players.Add(new PlayerPersistance { Id = PLAYER9, UserId = USER1, GameId = GAME_ID, GamePosition = 1 });
            DbContext.Players.Add(new PlayerPersistance { Id = PLAYER3, UserId = USER2, GameId = GAME_ID, GamePosition = 2 });
            DbContext.Players.Add(new PlayerPersistance { Id = PLAYER8, UserId = USER3, GameId = GAME_ID, GamePosition = 3 });
            DbContext.Players.Add(new PlayerPersistance { Id = PLAYER14, UserId = USER4, GameId = GAME_ID, GamePosition = 4 });
            DbContext.SaveChanges();
        }

        [Fact]
        public void ReturnApproximatelyEqualWhenSameNumberTilesToPlay()
        {
            List<Player> players = new List<Player> { Persistance.GetPlayerById(PLAYER9), Persistance.GetPlayerById(PLAYER3), Persistance.GetPlayerById(PLAYER8), Persistance.GetPlayerById(PLAYER14) };

            var player_count = new Dictionary<int, int> { { PLAYER9, 0 }, { PLAYER3, 0 }, { PLAYER8, 0 }, { PLAYER14, 0 } };
            for (int i = 0; i < 1000; i++)
                player_count[ComplianceService.InitGame(players)]++;

            Assert.True(200 < player_count[PLAYER9]);
            Assert.True(200 < player_count[PLAYER3]);
            Assert.True(200 < player_count[PLAYER8]);
            Assert.True(200 < player_count[PLAYER14]);
        }

        //[Fact]
        //public void ReturnGoodPlayerWhichStartTheGame()
        //{
        //    var player9 = DbContext.Players.Where(p => p.Id == PLAYER9).First();
        //    var player3 = DbContext.Players.Where(p => p.Id == PLAYER3).First();
        //    var player8 = DbContext.Players.Where(p => p.Id == PLAYER8).First();
        //    var player14 = DbContext.Players.Where(p => p.Id == PLAYER14).First();

        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 1, PlayerId = PLAYER9, TileId = 1 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 2, PlayerId = PLAYER9, TileId = 2 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 3, PlayerId = PLAYER9, TileId = 3 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 11, PlayerId = PLAYER3, TileId = 7 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 12, PlayerId = PLAYER3, TileId = 8 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 21, PlayerId = PLAYER8, TileId = 13 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 25, PlayerId = PLAYER14, TileId = 19 });
        //    DbContext.SaveChanges();

        //    List<Player> players = new List<Player> { Persistance.GetPlayerById(PLAYER9), Persistance.GetPlayerById(PLAYER3), Persistance.GetPlayerById(PLAYER8), Persistance.GetPlayerById(PLAYER14) };
        //    ComplianceService.SelectFirstPlayer(players);
        //    Assert.True(player9.GameTurn);

        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 31, PlayerId = PLAYER3, TileId = 9 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 32, PlayerId = PLAYER3, TileId = 10 });
        //    DbContext.SaveChanges();
        //    players = new List<Player> { Persistance.GetPlayerById(PLAYER9), Persistance.GetPlayerById(PLAYER3), Persistance.GetPlayerById(PLAYER8), Persistance.GetPlayerById(PLAYER14) };
        //    ComplianceService.SelectFirstPlayer(players);
        //    Assert.True(player3.GameTurn);

        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 131, PlayerId = PLAYER8, TileId = 14 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 132, PlayerId = PLAYER8, TileId = 15 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 133, PlayerId = PLAYER8, TileId = 16 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 134, PlayerId = PLAYER8, TileId = 17 });
        //    DbContext.SaveChanges();
        //    players = new List<Player> { Persistance.GetPlayerById(PLAYER9), Persistance.GetPlayerById(PLAYER3), Persistance.GetPlayerById(PLAYER8), Persistance.GetPlayerById(PLAYER14) };
        //    ComplianceService.SelectFirstPlayer(players);
        //    Assert.True(player8.GameTurn);

        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 231, PlayerId = PLAYER14, TileId = 20 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 232, PlayerId = PLAYER14, TileId = 21 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 233, PlayerId = PLAYER14, TileId = 22 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 234, PlayerId = PLAYER14, TileId = 23 });
        //    DbContext.TilesOnPlayer.Add(new TileOnPlayerPersistance { Id = 235, PlayerId = PLAYER14, TileId = 24 });
        //    DbContext.SaveChanges();
        //    players = new List<Player> { Persistance.GetPlayerById(PLAYER9), Persistance.GetPlayerById(PLAYER3), Persistance.GetPlayerById(PLAYER8), Persistance.GetPlayerById(PLAYER14) };
        //    ComplianceService.SelectFirstPlayer(players);
        //    Assert.True(player14.GameTurn);
        //}
    }
}
