using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistence.Adapters
{
    public class CompliancePersistenceAdapter : ICompliancePersistence
    {
        private DefaultDbContext DbContext { get; }

        private const int TOTAL_TILES = 108;

        public CompliancePersistenceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        public void CreateTiles(int gameId)
        {
            AddAllTilesInDataBase();

            var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
            for (int i = 0; i < TOTAL_TILES; i++)
                DbContext.TilesOnBag.Add(new TileOnBagPersistence { GameId = gameId, TileId = tilesIds[i] });

            DbContext.SaveChanges();
        }

        public Player CreatePlayer(int userId, int gameId)
        {
            var playerPersistence = new PlayerPersistence { GameId = gameId, UserId = userId };
            DbContext.Players.Add(playerPersistence);
            DbContext.SaveChanges();
            return PlayerPersistenceToPlayer(playerPersistence);
        }

        public Game CreateGame(DateTime date)
        {
            var game = new GamePersistence { CreatedDate = date };
            DbContext.Games.Add(game);
            DbContext.SaveChanges();
            return GamePersistenceToGame(game);
        }

        public Tile GetTileById(int tileId)
            => TilePersistenceToTile(DbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault());

        public Game GetGame(int gameId)
        {
            var gamePersistence = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            var tilesOnGamePersistence = DbContext.TilesOnGame.Where(tb => tb.GameId == gameId).ToList();
            var tiles = TilesOnGamePersistenceToTiles(tilesOnGamePersistence);
            var playersPersistence = DbContext.Players.Where(p => p.GameId == gameId).ToList();
            var players = new List<Player>();
            playersPersistence.ForEach(player => players.Add(PlayerPersistenceToPlayer(player)));
            var tilesOnBag = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
            Bag bag = new Bag { Id = gameId, Tiles = new List<Tile>() };
            tilesOnBag.ForEach(t => bag.Tiles.Add(TileOnBagToTile(t)));
            return new Game(gamePersistence.Id, tiles, players, bag);
        }

        public Player GetPlayer(int playerId)
            => PlayerPersistenceToPlayer(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());


        public void UpdatePlayer(Player player)
        {
            DbContext.Players.Update(PlayerToPlayerPersistence(player));
            DbContext.SaveChanges();
        }

        public void TilesFromBagToPlayer(Player player, int tilesNumber)
        {
            var tilesOnBag = DbContext.TilesOnBag.Where(t => t.GameId == player.GameId).OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
            DbContext.TilesOnBag.RemoveRange(tilesOnBag);
            tilesOnBag.ForEach(tb => DbContext.TilesOnPlayer.Add(TileOnBagToTileOnPlayer(tb, player.Id)));
            DbContext.SaveChanges();
        }

        public void TilesFromPlayerToBag(Player player, List<Tile> tiles)
        {
            var game = DbContext.Games.Where(g => g.Id == player.GameId).FirstOrDefault();
            game.LastPlayedDate = DateTime.Now;
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id && tiles.Select(t => t.Id).Contains(t.TileId)).ToList();
            DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayer);
            tilesOnPlayer.ForEach(tp => DbContext.TilesOnBag.Add(TileOnPlayerToTileOnBag(tp, player.GameId)));
            DbContext.SaveChanges();
        }

        public void TilesFromPlayerToGame(int gameId, int playerId, List<Tile> tiles)
        {
            var game = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            game.LastPlayedDate = DateTime.Now;
            tiles.ForEach(t => DbContext.TilesOnGame.Add(TileToTileOnGamePersistence(t, gameId)));
            tiles.ForEach(t => DbContext.TilesOnPlayer.Remove(DbContext.TilesOnPlayer.FirstOrDefault(tp => tp.TileId == t.Id && tp.PlayerId == playerId)));
            DbContext.SaveChanges();
        }

        public void SetPlayerTurn(int playerId, bool turn)
        {
            var player = DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault();
            player.GameTurn = turn;
            DbContext.SaveChanges();
        }

        public bool IsPlayerTurn(int playerId)
            => DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault().GameTurn;

        private void AddAllTilesInDataBase()
        {
            if (DbContext.Tiles.Count() == TOTAL_TILES) return;

            const int NUMBER_OF_SAME_TILE = 3;
            for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                    foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                        DbContext.Tiles.Add(new TilePersistence { Color = color, Form = form });

            DbContext.SaveChanges();
        }

        private List<Tile> TilesOnGamePersistenceToTiles(List<TileOnGamePersistence> tilesOnGame)
        {
            var tiles = new List<Tile>();
            var tilesPersistence = DbContext.Tiles.Where(t => tilesOnGame.Select(tb => tb.TileId).Contains(t.Id)).ToList();
            foreach (var tilePersistence in tilesPersistence)
            {
                var tileOnGame = tilesOnGame.FirstOrDefault(tb => tb.TileId == tilePersistence.Id);
                tiles.Add(new Tile(tilePersistence.Id, tilePersistence.Color, tilePersistence.Form, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
            }
            return tiles;
        }

        private PlayerPersistence PlayerToPlayerPersistence(Player player) // ! Ne retourne pas les Tiles
        {
            var gamePlayerPersistence = DbContext.Players.Where(gp => gp.Id == player.Id).FirstOrDefault();
            gamePlayerPersistence.Points = (byte)player.Points;
            gamePlayerPersistence.GameTurn = player.GameTurn;
            gamePlayerPersistence.GamePosition = (byte)player.GamePosition;
            return gamePlayerPersistence;
        }

        private Player PlayerPersistenceToPlayer(PlayerPersistence playerPersistence)
        {
            var player = new Player { Id = playerPersistence.Id, GameId = playerPersistence.GameId, GameTurn = playerPersistence.GameTurn, GamePosition = playerPersistence.GamePosition, Points = playerPersistence.Points };
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerPersistence.Id).Include(t => t.Tile).ToList();
            var tiles = new List<Tile>();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerPersistenceToTile(tp)));
            player.Tiles = tiles;
            return player;
        }
        private TileOnGamePersistence TileToTileOnGamePersistence(Tile tile, int gameId)
            => new TileOnGamePersistence { TileId = tile.Id, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };

        private TileOnPlayerPersistence TileOnBagToTileOnPlayer(TileOnBagPersistence tileOnBag, int playerId)
            => new TileOnPlayerPersistence { TileId = tileOnBag.TileId, PlayerId = playerId };

        private TileOnBagPersistence TileOnPlayerToTileOnBag(TileOnPlayerPersistence tileOnPlayer, int gameId)
            => new TileOnBagPersistence { TileId = tileOnPlayer.TileId, GameId = gameId };

        private Tile TileOnPlayerPersistenceToTile(TileOnPlayerPersistence tileOnPlayer)
            => new Tile(tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);

        private Tile TilePersistenceToTile(TilePersistence tilePersistence)
            => new Tile(tilePersistence.Id, tilePersistence.Color, tilePersistence.Form);

        private Game GamePersistenceToGame(GamePersistence game)
            => new Game(game.Id, TilesOnGamePersistenceToTiles(DbContext.TilesOnGame.Where(tb => tb.GameId == game.Id).ToList()), new List<Player>());

        private Tile TileOnBagToTile(TileOnBagPersistence tb)
            => new Tile(tb.Id, tb.Tile.Color, tb.Tile.Form, new CoordinatesInGame());

        private TileOnPlayerPersistence TileToTileOnPlayerPersistence(Tile tile, int playerId)
            => new TileOnPlayerPersistence { Id = tile.Id, TileId = tile.Id, PlayerId = playerId };
    }
}
