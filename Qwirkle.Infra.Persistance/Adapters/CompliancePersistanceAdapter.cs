using Qwirkle.Core.CommonContext;
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

        public Tile GetTileById(int tileId)
        {
            var tilePersistance = DbContext.Tiles.Where(t => t.Id == tileId).FirstOrDefault();
            return TilePersistanceToTileEntities(tilePersistance);
        }

        private Tile TilePersistanceToTileEntities(TilePersistance tilePersistance)
        {
            return new Tile(tilePersistance.Id, tilePersistance.Color, tilePersistance.Form);
        }

        public Board GetBoardByGameId(int boardId)
        {
            var gamePersistance = DbContext.Games.Where(g => g.Id == boardId).FirstOrDefault();
            var tilesOnBoardPersistance = DbContext.TilesOnBoard.Where(tb => tb.GameId == boardId).ToList();
            List<Tile> tiles = TilesOnBoardPersistanceToTilesEntities(tilesOnBoardPersistance);
            return new Board(gamePersistance.Id, tiles);
        }

        private List<Tile> TilesOnBoardPersistanceToTilesEntities(List<TileOnBoardPersistance> tilesOnBoard)
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

        public Player GetPlayerById(int playerId)
        {
            var player = PlayerPersistanceToPlayerEntities(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(x => x.PlayerId == playerId).ToList();
            var tiles = TilesOnPlayerPersistanceToTilesEntities(tilesOnPlayer);
            player.Tiles = tiles;
            return player;
        }

        private List<Tile> TilesOnPlayerPersistanceToTilesEntities(List<TileOnPlayerPersistance> tilesOnPlayer)
        {
            var tiles = new List<Tile>();
            foreach (var tileOnPlayer in tilesOnPlayer)
            {
                tiles.Add(TileOnPlayerPersistanceToTileEntities(tileOnPlayer));
            }
            return tiles;
        }

        private Tile TileOnPlayerPersistanceToTileEntities(TileOnPlayerPersistance tileOnPlayer)
        {
            return new Tile(tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);
        }

        public void UpdatePlayer(Player player)
        {
            DbContext.Players.Update(PlayerEntitiesToPlayerPersistance(player));
            DbContext.SaveChanges();
        }

        public void TilesFromBagToPlayer(Player player, int tilesNumber)
        {
            var tilesOnBag = DbContext.TilesOnBag.Where(t => t.GameId == 1).OrderBy(_ => Guid.NewGuid()).Take(tilesNumber).ToList(); //todo
            DbContext.TilesOnBag.RemoveRange(tilesOnBag);
            DbContext.TilesOnPlayer.AddRange(TilesOnBagToTilesOnPlayer(tilesOnBag, player.Id));
            DbContext.SaveChanges();
        }

        private List<TileOnPlayerPersistance> TilesOnBagToTilesOnPlayer(List<TileOnBagPersistance> tilesOnBag, int playerId)
        {
            List<TileOnPlayerPersistance> tilesOnPlayerPersistance = new List<TileOnPlayerPersistance>();
            foreach (var tileOnBag in tilesOnBag)
            {
                tilesOnPlayerPersistance.Add(TileOnBagToTileOnPlayer(tileOnBag, playerId));
            }
            return tilesOnPlayerPersistance;
        }

        private TileOnPlayerPersistance TileOnBagToTileOnPlayer(TileOnBagPersistance tileOnBag, int playerId)
        {
            return new TileOnPlayerPersistance { TileId = tileOnBag.TileId, PlayerId = playerId }; // todo position !
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
            DbContext.TilesOnBoard.AddRange(TilesEntitiesToTilesOnBoardPersistance(tiles, gameId));
            DbContext.SaveChanges();
        }

        private PlayerPersistance PlayerEntitiesToPlayerPersistance(Player player)
        {
            var gamePlayerPersistance = DbContext.Players.Where(gp => gp.Id == player.Id).FirstOrDefault();
            gamePlayerPersistance.Points = player.Points;
            gamePlayerPersistance.GameTurn = player.GameTurn;
            return gamePlayerPersistance;
        }

        private Player PlayerPersistanceToPlayerEntities(PlayerPersistance player)
            => new Player { Id = player.Id, GameId = player.GameId, GameTurn = player.GameTurn, GamePosition = player.GamePosition, Points = player.Points };

        private List<TileOnBoardPersistance> TilesEntitiesToTilesOnBoardPersistance(List<Tile> tiles, int gameId)
        {
            var tilesOnBoard = new List<TileOnBoardPersistance>();
            foreach (var tile in tiles)
            {
                tilesOnBoard.Add(TileEntitiesToTileOnBoardPersistance(tile, gameId));
            }
            return tilesOnBoard;
        }

        private TileOnBoardPersistance TileEntitiesToTileOnBoardPersistance(Tile tile, int gameId)
            => new TileOnBoardPersistance { TileId = tile.Id, GameId = gameId, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y };

        private List<TileOnPlayerPersistance> TilesEntitiesToTilesOnPlayerPersistance(List<Tile> tilesToRemove, int playerId)
        {
            var tilesOnPlayer = new List<TileOnPlayerPersistance>();
            foreach (var tile in tilesToRemove)
            {
                tilesOnPlayer.Add(TileEntitiesToTilesOnPlayerPersistance(tile, playerId));
            }
            return tilesOnPlayer;
        }

        private TileOnPlayerPersistance TileEntitiesToTilesOnPlayerPersistance(Tile tile, int playerId)
            => new TileOnPlayerPersistance { TileId = tile.Id, PlayerId = playerId };
    }
}
