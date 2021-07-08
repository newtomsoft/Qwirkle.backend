using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.Enums;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Dao;
using System;
using System.Collections.Generic;
using Xunit;

namespace Qwirkle.Core.Tests
{
    public class PlayTilesShould
    {
        private CoreUseCase ComplianceService { get; set; }
        private IRepository Repository { get; set; }

        private const int TOTAL_TILES = 108;
        private const int GAME_ID = 7;
        private const int USER71 = 71;
        private const int USER21 = 21;
        private const int USER3 = 3;
        private const int USER14 = 14;
        private const int PLAYER9 = 9;
        private const int PLAYER3 = 3;
        private const int PLAYER8 = 8;
        private const int PLAYER14 = 14;

        public PlayTilesShould()
        {
            Repository = new Repository(Context());
            ComplianceService = new CoreUseCase(Repository);
        }

        private DefaultDbContext Context()
        {
            var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new DefaultDbContext(contextOptions);
            AddAllTiles(dbContext);
            AddUsers(dbContext);
            AddGames(dbContext);
            AddPlayers(dbContext);
            AddTilesOnPlayers(dbContext);
            AddTilesOnBag(dbContext);
            return dbContext;
        }

        private void AddAllTiles(DefaultDbContext dbContext)
        {
            const int NUMBER_OF_SAME_TILE = 3;
            int id = 0;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        dbContext.Tiles.Add(new TileDao { Id = ++id, Color = color, Form = form });

            dbContext.SaveChanges();
        }

        private static void AddGames(DefaultDbContext dbContext)
        {
            dbContext.Games.Add(new GameDao { Id = GAME_ID, });
            dbContext.SaveChanges();
        }

        private static void AddUsers(DefaultDbContext dbContext)
        {
            dbContext.Users.Add(new UserDao { Id = USER71 });
            dbContext.Users.Add(new UserDao { Id = USER21 });
            dbContext.Users.Add(new UserDao { Id = USER3 });
            dbContext.Users.Add(new UserDao { Id = USER14 });
            dbContext.SaveChanges();
        }

        private static void AddPlayers(DefaultDbContext dbContext)
        {
            dbContext.Players.Add(new PlayerDao { Id = PLAYER9, UserId = USER71, GameId = GAME_ID, GamePosition = 1, GameTurn = true });
            dbContext.Players.Add(new PlayerDao { Id = PLAYER3, UserId = USER21, GameId = GAME_ID, GamePosition = 2, GameTurn = false });
            dbContext.Players.Add(new PlayerDao { Id = PLAYER8, UserId = USER3, GameId = GAME_ID, GamePosition = 3, GameTurn = false });
            dbContext.Players.Add(new PlayerDao { Id = PLAYER14, UserId = USER14, GameId = GAME_ID, GamePosition = 4, GameTurn = false });
            dbContext.SaveChanges();
        }

        private static void AddTilesOnPlayers(DefaultDbContext dbContext)
        {
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 1, PlayerId = PLAYER9, TileId = 1 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 2, PlayerId = PLAYER9, TileId = 2 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 3, PlayerId = PLAYER9, TileId = 3 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 4, PlayerId = PLAYER9, TileId = 4 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 5, PlayerId = PLAYER9, TileId = 5 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 6, PlayerId = PLAYER9, TileId = 6 });

            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 11, PlayerId = PLAYER3, TileId = 7 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 12, PlayerId = PLAYER3, TileId = 8 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 13, PlayerId = PLAYER3, TileId = 9 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 14, PlayerId = PLAYER3, TileId = 10 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 15, PlayerId = PLAYER3, TileId = 11 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 16, PlayerId = PLAYER3, TileId = 12 });

            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 21, PlayerId = PLAYER8, TileId = 13 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 22, PlayerId = PLAYER8, TileId = 14 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 23, PlayerId = PLAYER8, TileId = 15 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 24, PlayerId = PLAYER8, TileId = 16 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 25, PlayerId = PLAYER8, TileId = 17 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 26, PlayerId = PLAYER8, TileId = 18 });

            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 31, PlayerId = PLAYER14, TileId = 19 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 32, PlayerId = PLAYER14, TileId = 20 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 33, PlayerId = PLAYER14, TileId = 21 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 34, PlayerId = PLAYER14, TileId = 22 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 35, PlayerId = PLAYER14, TileId = 23 });
            dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 36, PlayerId = PLAYER14, TileId = 24 });

            dbContext.SaveChanges();
        }

        private void AddTilesOnBag(DefaultDbContext dbContext)
        {
            for (int i = 1; i <= TOTAL_TILES; i++)
                dbContext.TilesOnBag.Add(new TileOnBagDao { Id = 100 + i, GameId = GAME_ID, TileId = i });

            dbContext.SaveChanges();
        }

        [Fact]
        public void Return3After1PlayerHavePlayedHisTiles()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
            Assert.Equal(3, ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay).Points);
        }

        [Fact]
        public void Return0WhenItsNotTurnPlayer()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
            Assert.Equal(0, ComplianceService.TryPlayTiles(PLAYER3, tilesToPlay).Points);
        }

        [Fact]
        public void Return0After1PlayerHavePlayedNotHisTiles()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (7, -3, 4) };
            Assert.Equal(0, ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay).Points);
        }

        [Fact]
        public void Return5After2PlayersHavePlayed()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
            ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay);

            var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
            Assert.Equal(5, ComplianceService.TryPlayTiles(PLAYER3, tilesToPlay2).Points);
        }

        [Fact]
        public void Return6After3PlayersHavePlayed()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
            ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay);
            var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
            ComplianceService.TryPlayTiles(PLAYER3, tilesToPlay2);

            var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
            Assert.Equal(6, ComplianceService.TryPlayTiles(PLAYER8, tilesToPlay3).Points);
        }

        [Fact]
        public void Return6After4PlayersHavePlayed()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
            ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay);
            var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
            ComplianceService.TryPlayTiles(PLAYER3, tilesToPlay2);
            var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
            ComplianceService.TryPlayTiles(PLAYER8, tilesToPlay3);

            var tilesToPlay4 = new List<(int tileId, sbyte x, sbyte y)> { (21, -3, 2), (20, -3, 1), (19, -3, 0) };
            Assert.Equal(6, ComplianceService.TryPlayTiles(PLAYER14, tilesToPlay4).Points);
        }

        [Fact]
        public void Return0AfterPlayersHavePlayedOnTheSamePlaceThanOtherTile()
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
            ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay);
            var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
            ComplianceService.TryPlayTiles(PLAYER3, tilesToPlay2);
            var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
            ComplianceService.TryPlayTiles(PLAYER8, tilesToPlay3);
            var tilesToPlay4 = new List<(int tileId, sbyte x, sbyte y)> { (21, -3, 2), (20, -3, 1), (19, -3, 0) };
            ComplianceService.TryPlayTiles(PLAYER14, tilesToPlay4);

            var tilesToPlay5 = new List<(int tileId, sbyte x, sbyte y)> { (4, -3, 2), (5, -3, 1), (6, -3, 0) };
            Assert.Equal(0, ComplianceService.TryPlayTiles(PLAYER9, tilesToPlay5).Points);
        }

    }
}
