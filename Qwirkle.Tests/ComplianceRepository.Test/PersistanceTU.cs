using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.Ports;
using Qwirkle.Core.ValueObjects;
using Qwirkle.Infra.Repository;
using Qwirkle.Infra.Repository.Adapters;
using Qwirkle.Infra.Repository.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Qwirkle.Core.ComplianceRepository.Tests
{
    public class RepositoryTU
    {
        #region private attributs
        private IRepository Repository { get; set; }
        private DefaultDbContext DbContext { get; set; }

        private const int USER1 = 71;
        private const int USER2 = 21;
        private const int USER3 = 3;
        private const int USER4 = 14;
        #endregion

        public RepositoryTU()
        {
            var factory = new ConnectionFactory();
            DbContext = factory.CreateContextForInMemory();
            Repository = new Repository(DbContext);
            AddUsers();
            AddGame();
            AddAllTiles();
        }

        #region private methods
        private void AddUsers()
        {
            DbContext.Users.Add(new UserDao { Id = USER1 });
            DbContext.Users.Add(new UserDao { Id = USER2 });
            DbContext.Users.Add(new UserDao { Id = USER3 });
            DbContext.Users.Add(new UserDao { Id = USER4 });
            DbContext.SaveChanges();
        }

        private void AddGame()
        {
            DbContext.Games.Add(new GameDao { CreatDate = DateTime.Now });
            DbContext.SaveChanges();
        }

        private void AddAllTiles()
        {
            const int NUMBER_OF_SAME_TILE = 3;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        DbContext.Tiles.Add(new TileDao { Color = color, Form = form });

            DbContext.SaveChanges();
        }

        private void AddTilesOnBag(int gameId, int number)
        {
            var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
            for (int i = 1; i <= number; i++)
            {
                DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });
            }
            DbContext.SaveChanges();
        }
        #endregion

        [Fact]
        public void CreatePlayerShould()
        {
            var game = DbContext.Games.First();
            var player = Repository.CreatePlayer(USER1, game.Id);
            Assert.Equal(game.Id, player.GameId);
            Assert.Equal(0, player.GamePosition);
            Assert.False(player.IsTurn);
            Assert.Equal(0, player.Points);
            Assert.Empty(player.Rack.Tiles);
        }

        [Fact]
        public void CreateGameShould()
        {
            var game = Repository.CreateGame(DateTime.Today);
            Assert.Empty(game.Board.Tiles);
            Assert.Empty(game.Players);
        }

        [Fact]
        public void UpdatePlayerShould()
        {
            byte points = 10;
            byte position = 2;
            var game = Repository.CreateGame(DateTime.Today);
            var player1 = Repository.CreatePlayer(USER1, game.Id);
            player1.Points = points;
            player1.SetTurn(true);
            player1.GamePosition = position;
            Repository.UpdatePlayer(player1);

            var playerUpdate = Repository.GetPlayer(player1.Id);
            Assert.Equal(points, playerUpdate.Points);
            Assert.True(playerUpdate.IsTurn);
            Assert.Equal(position, playerUpdate.GamePosition);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldNotGiveTileIfBagIsEmpty()
        {
            var game = Repository.CreateGame(DateTime.Today);
            var player = Repository.CreatePlayer(USER1, game.Id);
            Repository.TilesFromBagToPlayer(player, new List<byte> { 0, 1, 2, 3, 4, 5 });
            var playerUpdate = Repository.GetPlayer(player.Id);
            Assert.Empty(playerUpdate.Rack.Tiles);
            Assert.Empty(playerUpdate.Rack.Tiles);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldGiveTilesIfBagContainEnoughTiles()
        {
            int TilesNumberToAdd = 3;
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TilesNumberToAdd; i++)
            {
                rackPositions.Add(i);
            }
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAdd);
            var player = Repository.CreatePlayer(USER1, game.Id);
            Repository.TilesFromBagToPlayer(player, rackPositions);
            var playerUpdate = Repository.GetPlayer(player.Id);
            Assert.Equal(TilesNumberToAdd, playerUpdate.Rack.Tiles.Count);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldGive4TilesWhenRequest2x2()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequest = 2;
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TilesNumberToRequest; i++)
            {
                rackPositions.Add(i);
            }
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAddInBag);
            var player = Repository.CreatePlayer(USER1, game.Id);
            Repository.TilesFromBagToPlayer(player, rackPositions);
            Repository.TilesFromBagToPlayer(player, rackPositions);
            var playerUpdate = Repository.GetPlayer(player.Id);
            Assert.Equal(TilesNumberToRequest + TilesNumberToRequest, playerUpdate.Rack.Tiles.Count);
        }

        [Fact]
        public void TilesFromPlayerToBagAllShouldEmptyPlayer()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequestFromBag = 4;
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TilesNumberToRequestFromBag; i++)
            {
                rackPositions.Add(i);
            }
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAddInBag);
            var player = Repository.CreatePlayer(USER1, game.Id);
            Repository.TilesFromBagToPlayer(player, rackPositions);
            var playerAfterDraw = Repository.GetPlayer(player.Id);
            Repository.TilesFromPlayerToBag(playerAfterDraw, playerAfterDraw.Rack.Tiles);
            var playerUpdate = Repository.GetPlayer(player.Id);
            var gameUpdate = Repository.GetGame(game.Id);
            Assert.Empty(playerUpdate.Rack.Tiles);
            Assert.Equal(TilesNumberToAddInBag, gameUpdate.Bag.Tiles.Count);
        }

        [Fact]
        public void GetGameShouldContainPlayer()
        {
            int TilesNumberToAddInBag = 10;
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAddInBag);
            var player1 = Repository.CreatePlayer(USER1, game.Id);
            var player2 = Repository.CreatePlayer(USER2, game.Id);
            var player3 = Repository.CreatePlayer(USER3, game.Id);
            var player4 = Repository.CreatePlayer(USER4, game.Id);
            var gameUpdate = Repository.GetGame(game.Id);
            Assert.Contains(player1.Id, gameUpdate.Players.Select(p => p.Id));
            Assert.Contains(player2.Id, gameUpdate.Players.Select(p => p.Id));
            Assert.Contains(player3.Id, gameUpdate.Players.Select(p => p.Id));
            Assert.Contains(player4.Id, gameUpdate.Players.Select(p => p.Id));
        }

        [Fact]
        public void GetGameShouldContainBag()
        {
            int TilesNumberToAddInBag = 10;
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAddInBag);
            var gameUpdate = Repository.GetGame(game.Id);
            Assert.Equal(TilesNumberToAddInBag, gameUpdate.Bag.Tiles.Count);
        }

        [Fact]
        public void TilesFromPlayerToGameShould()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequestFromBag = 4;
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TilesNumberToRequestFromBag; i++)
            {
                rackPositions.Add(i);
            }
            var game = Repository.CreateGame(DateTime.Today);
            AddTilesOnBag(game.Id, TilesNumberToAddInBag);
            var player = Repository.CreatePlayer(USER1, game.Id);
            Repository.TilesFromBagToPlayer(player, rackPositions);
            player = Repository.GetPlayer(player.Id);
            var coordinates = new CoordinatesInGame(2, 5);
            var tile = new TileOnBoard(player.Rack.Tiles[0], coordinates);
            Repository.TilesFromPlayerToGame(game.Id, player.Id, new List<TileOnBoard> { tile });
            var gameUpdate = Repository.GetGame(game.Id);
            Assert.Single(gameUpdate.Board.Tiles);
            Assert.Equal(coordinates, gameUpdate.Board.Tiles[0].Coordinates);
        }

    }
}
