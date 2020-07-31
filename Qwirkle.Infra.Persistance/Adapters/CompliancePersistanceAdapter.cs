using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class CompliancePersistanceAdapter : ICompliancePersistance
    {
        private DefaultDbContext DbContext { get; }

        public CompliancePersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        public Player CreatePlayer(int userId, int gameId)
        {
            var playerPersistance = new PlayerPersistance { GameId = gameId, UserId = userId };
            DbContext.Players.Add(playerPersistance);
            DbContext.SaveChanges();
            return PlayerPersistanceToPlayer(playerPersistance);
        }

        public Board CreateBoard(DateTime date)
        {
            var game = new GamePersistance { CreatedDate = date };
            DbContext.Games.Add(game);
            DbContext.SaveChanges();
            return GamePersistanceToBoard(game);
        }

        public Tile GetTileById(int tileId)
        {
            var tilePersistance = DbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
            return TilePersistanceToTile(tilePersistance);
        }

        public Board GetBoardByGameId(int boardId)
        {
            var gamePersistance = DbContext.Games.Where(g => g.Id == boardId).FirstOrDefault();
            var tilesOnBoardPersistance = DbContext.TilesOnBoard.Where(tb => tb.GameId == boardId).ToList();
            var tiles = TilesOnBoardPersistanceToTiles(tilesOnBoardPersistance);
            var playersPersistance = DbContext.Players.Where(p => p.GameId == boardId).ToList();
            var players = new List<Player>();
            playersPersistance.ForEach(player => players.Add(PlayerPersistanceToPlayer(player)));
            return new Board(gamePersistance.Id, tiles, players);
        }

        public Player GetPlayerById(int playerId)
        {
            var player = PlayerPersistanceToPlayer(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(x => x.PlayerId == playerId).ToList();
            player.Tiles = new List<Tile>();
            tilesOnPlayer.ForEach(tp => player.Tiles.Add(TileOnPlayerPersistanceToTile(tp)));
            return player;
        }

        public void UpdatePlayer(Player player)
        {
            DbContext.Players.Update(PlayerToPlayerPersistance(player));
            DbContext.SaveChanges();
        }

        public void TilesFromBagToPlayer(Player player, int tilesNumber)
        {
            var tilesOnBag = DbContext.TilesOnBag.Where(t => t.GameId == player.GameId).OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
            DbContext.TilesOnBag.RemoveRange(tilesOnBag);
            tilesOnBag.ForEach(tb => DbContext.TilesOnPlayer.Add(TileOnBagToTileOnPlayer(tb, player.Id)));
            DbContext.SaveChanges();
        }

        public void TilesFromPlayerToBag(Player player, int tilesNumber)
        {
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id).OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList();
            DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayer);
            tilesOnPlayer.ForEach(tp => DbContext.TilesOnBag.Add(TileOnPlayerToTileOnBag(tp, player.GameId)));
            DbContext.SaveChanges();
        }

        public void TilesFromPlayerToBag(Player player, List<Tile> tiles)
        {
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(t => t.PlayerId == player.Id && tiles.Select(t => t.Id).Contains(t.Id)).ToList();
            DbContext.TilesOnPlayer.RemoveRange(tilesOnPlayer);
            tilesOnPlayer.ForEach(tp => DbContext.TilesOnBag.Add(TileOnPlayerToTileOnBag(tp, player.GameId)));
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

        public void TilesFromPlayerToBoard(int gameId, List<Tile> tiles)
        {
            var game = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            game.LastPlayedDate = DateTime.Now;
            tiles.ForEach(t => DbContext.TilesOnBoard.Add(TileToTileOnBoardPersistance(t, gameId)));
            DbContext.SaveChanges();
        }
        private List<Tile> TilesOnBoardPersistanceToTiles(List<TileOnBoardPersistance> tilesOnBoard)
        {
            var tiles = new List<Tile>();
            var tilesPersistance = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
            foreach (var tilePersistance in tilesPersistance)
            {
                var tileOnBoard = tilesOnBoard.FirstOrDefault(tb => tb.TileId == tilePersistance.Id);
                tiles.Add(new Tile(tilePersistance.Id, tilePersistance.Color, tilePersistance.Form, new CoordinatesInBoard(tileOnBoard.PositionX, tileOnBoard.PositionY)));
            }
            return tiles;
        }

        /// <summary>
        /// Convertion d'un Player en un PlayerPersistance.
        /// Ne retourne pas les Tiles
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private PlayerPersistance PlayerToPlayerPersistance(Player player)
        {
            var gamePlayerPersistance = DbContext.Players.Where(gp => gp.Id == player.Id).FirstOrDefault();
            gamePlayerPersistance.Points = player.Points;
            gamePlayerPersistance.GameTurn = player.GameTurn;
            gamePlayerPersistance.GamePosition = player.GamePosition;
            return gamePlayerPersistance;
        }

        private Player PlayerPersistanceToPlayer(PlayerPersistance playerPersistance)
        {
            var player = new Player { Id = playerPersistance.Id, GameId = playerPersistance.GameId, GameTurn = playerPersistance.GameTurn, GamePosition = playerPersistance.GamePosition, Points = playerPersistance.Points };
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerPersistance.Id).ToList();
            var tiles = new List<Tile>();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerPersistanceToTile(tp)));
            player.Tiles = tiles;
            return player;
        }
        private TileOnBoardPersistance TileToTileOnBoardPersistance(Tile tile, int gameId)
            => new TileOnBoardPersistance { TileId = tile.Id, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };

        private TileOnPlayerPersistance TileOnBagToTileOnPlayer(TileOnBagPersistance tileOnBag, int playerId)
            => new TileOnPlayerPersistance { TileId = tileOnBag.TileId, PlayerId = playerId }; // todo position !

        private TileOnBagPersistance TileOnPlayerToTileOnBag(TileOnPlayerPersistance tileOnPlayer, int gameId)
        {
            return new TileOnBagPersistance { TileId = tileOnPlayer.TileId, GameId = gameId };
        }

        private Tile TileOnPlayerPersistanceToTile(TileOnPlayerPersistance tileOnPlayer)
            => new Tile(tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);

        private Tile TilePersistanceToTile(TilePersistance tilePersistance)
            => new Tile(tilePersistance.Id, tilePersistance.Color, tilePersistance.Form);

        private Board GamePersistanceToBoard(GamePersistance game)
        {
            var tilesOnBoard = DbContext.TilesOnBoard.Where(tb => tb.GameId == game.Id).ToList();
            return new Board(game.Id, TilesOnBoardPersistanceToTiles(tilesOnBoard), new List<Player>());
        }

        private TileOnPlayerPersistance TileToTilesOnPlayerPersistance(Tile tile, int playerId)
            => new TileOnPlayerPersistance { TileId = tile.Id, PlayerId = playerId };
    }
}
