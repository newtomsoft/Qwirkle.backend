using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistance;
using Qwirkle.Infra.Persistance.Adapters;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Qwirkle.Core.CompliancePersistance.Tests
{
    public class PersistanceTU
    {
        private ICompliancePersistance Persistance { get; set; }
        private DefaultDbContext DbContext { get; set; }

        private const int USER1 = 71;
        private const int USER2 = 21;
        private const int USER3 = 3;
        private const int USER4 = 14;

        public PersistanceTU()
        {
            var factory = new ConnectionFactory();
            DbContext = factory.CreateContextForInMemory();
            Persistance = new CompliancePersistanceAdapter(DbContext);
            AddUsers();
            AddGame();
            AddAllTiles();
        }
        private void AddUsers()
        {
            DbContext.Users.Add(new UserPersistance { Id = USER1 });
            DbContext.Users.Add(new UserPersistance { Id = USER2 });
            DbContext.Users.Add(new UserPersistance { Id = USER3 });
            DbContext.Users.Add(new UserPersistance { Id = USER4 });
            DbContext.SaveChanges();
        }

        private void AddGame()
        {
            DbContext.Games.Add(new GamePersistance { CreatedDate = DateTime.Now });
            DbContext.SaveChanges();
        }

        private void AddAllTiles()
        {
            const int NUMBER_OF_SAME_TILE = 3;
            int id = 0;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        DbContext.Tiles.Add(new TilePersistance { Color = color, Form = form });

            DbContext.SaveChanges();
        }

        private void AddTilesOnBag(int gameId, int number)
        {
            var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
            for (int i = 1; i <= number; i++)
            {
                DbContext.TilesOnBag.Add(new TileOnBagPersistance {GameId = gameId, TileId = tilesIds[i] });
            }
            DbContext.SaveChanges();
        }

        [Fact]
        public void CreatePlayerShould()
        {
            var game = DbContext.Games.First();
            var player = Persistance.CreatePlayer(USER1, game.Id);
            Assert.Equal(game.Id, player.GameId);
            Assert.Equal(0, player.GamePosition);
            Assert.False(player.GameTurn);
            Assert.Equal(0, player.Points);
            Assert.Empty(player.Tiles);
        }

        [Fact]
        public void CreateBoardShould()
        {
            var board = Persistance.CreateBoard(DateTime.Today);
            Assert.Empty(board.Tiles);
            Assert.Empty(board.Players);
        }

        [Fact]
        public void UpdatePlayerShould()
        {
            byte points = 10;
            byte position = 2;
            var board = Persistance.CreateBoard(DateTime.Today);
            var player1 = Persistance.CreatePlayer(USER1, board.Id);
            player1.Points = points;
            player1.GameTurn = true;
            player1.GamePosition = position;
            Persistance.UpdatePlayer(player1);

            var playerUpdate = Persistance.GetPlayerById(player1.Id);
            Assert.Equal(points, playerUpdate.Points);
            Assert.True(playerUpdate.GameTurn);
            Assert.Equal(position, playerUpdate.GamePosition);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldNotGiveTileIfBagIsEmpty()
        {
            var board = Persistance.CreateBoard(DateTime.Today);
            var player = Persistance.CreatePlayer(USER1, board.Id);
            Persistance.TilesFromBagToPlayer(player, 6);
            var playerUpdate = Persistance.GetPlayerById(player.Id);
            Assert.Empty(playerUpdate.Tiles);
            Assert.Empty(playerUpdate.Tiles);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldGiveTilesIfBagContainEnoughTiles()
        {
            int TilesNumberToAdd = 3;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAdd);
            var player = Persistance.CreatePlayer(USER1, board.Id);
            Persistance.TilesFromBagToPlayer(player, TilesNumberToAdd);
            var playerUpdate = Persistance.GetPlayerById(player.Id);
            Assert.Equal(TilesNumberToAdd, playerUpdate.Tiles.Count);
        }

        [Fact]
        public void TilesFromBagToPlayerShouldGive4TilesWhenRequest2x2()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequest = 2;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAddInBag);
            var player = Persistance.CreatePlayer(USER1, board.Id);
            Persistance.TilesFromBagToPlayer(player, TilesNumberToRequest);
            Persistance.TilesFromBagToPlayer(player, TilesNumberToRequest);
            var playerUpdate = Persistance.GetPlayerById(player.Id);
            Assert.Equal(TilesNumberToRequest + TilesNumberToRequest, playerUpdate.Tiles.Count);
        }

        [Fact]
        public void TilesFromPlayerToBagShouldEmptyPlayer()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequest = 2;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAddInBag);
            var player = Persistance.CreatePlayer(USER1, board.Id);
            Persistance.TilesFromBagToPlayer(player, TilesNumberToRequest);
            Persistance.TilesFromPlayerToBag(player, TilesNumberToRequest);
            var playerUpdate = Persistance.GetPlayerById(player.Id);
            Assert.Empty(playerUpdate.Tiles);
        }

        [Fact]
        public void GetBoardByGameIdShouldContainPlayer()
        {
            int TilesNumberToAddInBag = 10;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAddInBag);
            var player1 = Persistance.CreatePlayer(USER1, board.Id);
            var player2 = Persistance.CreatePlayer(USER2, board.Id);
            var player3 = Persistance.CreatePlayer(USER3, board.Id);
            var player4 = Persistance.CreatePlayer(USER4, board.Id);
            var boardUpdate = Persistance.GetBoardByGameId(board.Id);
            Assert.Contains(player1.Id, boardUpdate.Players.Select(p => p.Id));
            Assert.Contains(player2.Id, boardUpdate.Players.Select(p => p.Id));
            Assert.Contains(player3.Id, boardUpdate.Players.Select(p => p.Id));
            Assert.Contains(player4.Id, boardUpdate.Players.Select(p => p.Id));
        }
        [Fact]
        public void GetBoardByGameIdShouldContainPlayer2()
        {
            int TilesNumberToAddInBag = 10;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAddInBag);
            var boardUpdate = Persistance.GetBoardByGameId(board.Id);
            Assert.Equal(TilesNumberToAddInBag, boardUpdate.Tiles.Count);
        }

        [Fact]
        public void TilesFromPlayerToBagShouldEmptyPlayer2()
        {
            int TilesNumberToAddInBag = 10;
            int TilesNumberToRequestFromBag = 2;
            int TilesNumberToRequestFromPlayer = 3;
            var board = Persistance.CreateBoard(DateTime.Today);
            AddTilesOnBag(board.Id, TilesNumberToAddInBag);
            var player = Persistance.CreatePlayer(USER1, board.Id);
            Persistance.TilesFromBagToPlayer(player, TilesNumberToRequestFromBag);
            Persistance.TilesFromPlayerToBag(player, TilesNumberToRequestFromPlayer);
            var playerUpdate = Persistance.GetPlayerById(player.Id);
            //var boardUpdate = Persistance.GetBoardByGameId(board.Id);
            Assert.Empty(playerUpdate.Tiles);
            //Assert.Equal(TilesNumberToAddInBag, boardUpdate.Tiles.Count);
        }


        
    }
}
