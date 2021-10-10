using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.Ports;
using Qwirkle.Core.ValueObjects;
using Qwirkle.Infra.Repository.Dao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Repository.Adapters
{
    public class Repository : IRepository
    {
        private DefaultDbContext DbContext { get; }

        private const int TOTAL_TILES = 108;

        public Repository(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        public void CreateTiles(int gameId)
        {
            AddAllTilesInDataBase();
            var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
            for (int i = 0; i < TOTAL_TILES; i++)
                DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });

            DbContext.SaveChanges();
        }

        public Player CreatePlayer(int userId, int gameId)
        {
            var playerDao = new PlayerDao { GameId = gameId, UserId = userId, LastTurnSkipped = false };
            DbContext.Players.Add(playerDao);
            DbContext.SaveChanges();
            return PlayerDaoToPlayer(playerDao);
        }

        public Game CreateGame(DateTime date)
        {
            var game = new GameDao { CreatDate = date };
            DbContext.Games.Add(game);
            DbContext.SaveChanges();
            return GameDaoToGame(game);
        }
        public List<int> GetListGameIDWithPlayer()
        {
            var listGame = DbContext.Players.Select(p => p.GameId).Distinct().ToList();
            return listGame;
        }
        public List<string> GetListNamePlayer(int gameId)
        {
            var listName = new List<string>();
            var listGame = DbContext.Players.Where(p => p.GameId == gameId).ToList();
            listGame.ForEach(player => listName.Add(DbContext.Users.Where(p => p.Id == player.UserId).Select(p => p.FirstName).FirstOrDefault()));
            return listName;
        }

        public string GetPlayerNameTurn(int gameId)
        {
            var userId = DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.UserId).FirstOrDefault();
            var namePlayerTurn = DbContext.Users.Where(p => p.Id == userId).Select(p => p.FirstName).FirstOrDefault();
            return namePlayerTurn;
        }

        public int GetPlayerIdToPlay(int gameId)
        {
            var playerId = DbContext.Players.Where(p => p.GameId == gameId && p.GameTurn).Select(p => p.Id).FirstOrDefault();
            return playerId;
        }


        public Tile GetTileById(int tileId)
            => TileDaoToTile(DbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault());

        public TileOnPlayer GetTileOnPlayerById(int tileId)
            => TileOnPlayerDaoToEntity(DbContext.TilesOnPlayer.Where(t => t.TileId == tileId).FirstOrDefault());

        public Game GetGame(int gameId)
        {
            var gameDao = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            var tilesOnGameDao = DbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToList();
            var tiles = TilesOnBoardDaoToEntity(tilesOnGameDao);
            var playersDao = DbContext.Players.Where(p => p.GameId == gameId).ToList();
            var players = new List<Player>();
            playersDao.ForEach(player => players.Add(PlayerDaoToPlayer(player)));
            var tilesOnBagDao = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
            var bag = new Bag(gameId);
            tilesOnBagDao.ForEach(t => bag.Tiles.Add(TileOnBagDaoToEntity(t)));
            return new Game(gameDao.Id, tiles, players, gameDao.GameOver, bag);
        }

        public Player GetPlayer(int playerId)
            => PlayerDaoToPlayer(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());

        //public List<TileOnPlayer> GetTilesOnPlayerByPlayerId(int playerId)
        //{
        //    var tilesOnPlayerModel = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerId).ToList();
        //    var tilesOnPlayer = new List<TileOnPlayer>();
        //    foreach (var tileOnPlayer in tilesOnPlayerModel)
        //        tilesOnPlayer.Add(TileOnPlayerModelToEntity(tileOnPlayer));

        //    return tilesOnPlayer;
        //}

        public void UpdatePlayer(Player player)
        {
            DbContext.Players.Update(PlayerToPlayerDao(player));
            DbContext.SaveChanges();
        }

#warning todo à vérifier rackposition !
        public void TilesFromBagToPlayer(Player player, List<byte> rackPositions)
        {
            int tilesNumber = rackPositions.Count;
            var tilesToGiveToPlayer = DbContext.TilesOnBag.Where(t => t.GameId == player.GameId).ToList().OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
            DbContext.TilesOnBag.RemoveRange(tilesToGiveToPlayer);
            for (int i = 0; i < tilesToGiveToPlayer.Count; i++)
            {
                DbContext.TilesOnPlayer.Add(new TileOnPlayerDao(tilesToGiveToPlayer[i], rackPositions[i], player.Id));
            }
            //tilesToGiveToPlayer.ForEach(tb => DbContext.TilesOnPlayer.Add(TileOnBagToTileOnPlayer(tb, player.Id))); // faire avec forEach pour prendre en compte rackPositions[i]
            DbContext.SaveChanges();
        }
        //todo rackPosition

        public void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles)
        {
            var game = DbContext.Games.Where(g => g.Id == player.GameId).FirstOrDefault();
            game.LastPlayDate = DateTime.Now;
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id && tiles.Select(t => t.Id).Contains(t.TileId)).ToList();
            DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayer);
            tilesOnPlayer.ForEach(tp => DbContext.TilesOnBag.Add(TileOnPlayerDaoToTileOnBagDao(tp, player.GameId)));
            DbContext.SaveChanges();
        }

        public void TilesFromPlayerToGame(int gameId, int playerId, List<TileOnBoard> tiles)
        {
            var game = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            game.LastPlayDate = DateTime.Now;
            tiles.ForEach(t => DbContext.TilesOnBoard.Add(TileToTileOnBoardDao(t, gameId)));
            tiles.ForEach(t => DbContext.TilesOnPlayer.Remove(DbContext.TilesOnPlayer.FirstOrDefault(tp => tp.TileId == t.Id && tp.PlayerId == playerId)));
            DbContext.SaveChanges();
        }

        public void SetPlayerTurn(int playerId, bool turn)
        {
            var player = DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault();
            player.GameTurn = turn;
            DbContext.SaveChanges();
        }

        public void SetGameOver(int gameId)
        {
            foreach (var player in DbContext.Players.Where(p => p.GameId == gameId))
                player.GameTurn = false;

            DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault().GameOver = true;
            DbContext.SaveChanges();
        }

        public bool IsPlayerTurn(int playerId)
            => DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault().GameTurn;

        private void AddAllTilesInDataBase()
        {
            if (DbContext.Tiles.Count() == TOTAL_TILES) return;

            const int NUMBER_OF_SAME_TILE = 3;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (var form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        DbContext.Tiles.Add(new TileDao { Color = color, Form = form });

            DbContext.SaveChanges();
        }

        private List<TileOnBoard> TilesOnBoardDaoToEntity(List<TileOnBoardDao> tilesOnBoard)
        {
            var tiles = new List<TileOnBoard>();
            var tilesModel = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
            foreach (var tileModel in tilesModel)
            {
                var tileOnGame = tilesOnBoard.FirstOrDefault(tb => tb.TileId == tileModel.Id);
                tiles.Add(new TileOnBoard(tileModel.Id, tileModel.Color, tileModel.Form, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
            }
            return tiles;
        }

        private PlayerDao PlayerToPlayerDao(Player player) // ! Ne retourne pas les Tiles
        {
            var playerDao = DbContext.Players.Where(gp => gp.Id == player.Id).FirstOrDefault();
            playerDao.Points = (byte)player.Points;
            playerDao.GameTurn = player.IsTurn;
            playerDao.GamePosition = (byte)player.GamePosition;
            playerDao.LastTurnSkipped = player.LastTurnSkipped;
            return playerDao;
        }

        private Player PlayerDaoToPlayer(PlayerDao playerDao)
        {
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerDao.Id).Include(t => t.Tile).ToList();
            var tiles = new List<TileOnPlayer>();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerDaoToEntity(tp)));
            var player = new Player(playerDao.Id, playerDao.GameId, playerDao.GamePosition, playerDao.Points, tiles, playerDao.GameTurn, playerDao.LastTurnSkipped);
            return player;
        }

        private static TileOnBoardDao TileToTileOnBoardDao(TileOnBoard tile, int gameId) => new() { TileId = tile.Id, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };

        private TileOnPlayerDao TileOnBagToTileOnPlayer(TileOnBagDao tileOnBag, int playerId) => new() { TileId = tileOnBag.TileId, PlayerId = playerId };

        private static TileOnBagDao TileOnPlayerDaoToTileOnBagDao(TileOnPlayerDao tileOnPlayer, int gameId) => new() { TileId = tileOnPlayer.TileId, GameId = gameId };

        private static TileOnPlayer TileOnPlayerDaoToEntity(TileOnPlayerDao tileOnPlayer) => new(tileOnPlayer.RackPosition, tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);

        private static Tile TileDaoToTile(TileDao tileModel) => new(tileModel.Id, tileModel.Color, tileModel.Form);

        private Game GameDaoToGame(GameDao game) => new(game.Id, TilesOnBoardDaoToEntity(DbContext.TilesOnBoard.Where(tb => tb.GameId == game.Id).ToList()), new List<Player>(), game.GameOver);

        private static TileOnBag TileOnBagDaoToEntity(TileOnBagDao tb) => new(tb.Id, tb.Tile.Color, tb.Tile.Form);

        private TileOnPlayerDao TileToTileOnPlayerModel(TileOnBag tile, int playerId) => new() { Id = tile.Id, TileId = tile.Id, PlayerId = playerId };

    }
}
