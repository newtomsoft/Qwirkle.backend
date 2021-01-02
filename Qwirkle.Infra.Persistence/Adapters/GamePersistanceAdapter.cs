using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext.Entities;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Infra.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistence.Adapters
{
    public class GamePersistenceAdapter : IGamePersistence
    {
        private DefaultDbContext DbContext { get; }


        public GamePersistenceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        public Game GetGame(int gameId)
        {
            var game = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();

            var tilesOnBoardModel = DbContext.TilesOnBoard.Where(tb => tb.GameId == gameId).ToList();
            var tilesOnBoard = TilesOnBoardModelToEntity(tilesOnBoardModel);

            var playersModel = DbContext.Players.Where(p => p.GameId == gameId).ToList();
            List<Player> players = new List<Player>();
            playersModel.ForEach(p => players.Add(PlayerModelToEntity(p)));

            var tilesOnBag = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
            Bag bag = new Bag(gameId);
            tilesOnBag.ForEach(t => bag.Tiles.Add(TileOnBagModelToEntity(t)));

            return new Game(game.Id, tilesOnBoard, players, bag);
        }

        private Player PlayerModelToEntity(PlayerModel p)
        {
            var tilesOnPlayerModel = DbContext.TilesOnPlayer.Where(t => t.PlayerId == p.Id).ToList();
            var tilesOnPlayer = new List<TileOnPlayer>();
            tilesOnPlayerModel.ForEach(t => tilesOnPlayer.Add(TileOnPlayerModelToEntity(t)));

            return new Player(p.Id, p.GameId, p.GamePosition, p.Points, tilesOnPlayer, p.GameTurn);
        }

        private TileOnPlayer TileOnPlayerModelToEntity(TileOnPlayerModel t)
         => new TileOnPlayer(t.RackPosition, t.TileId, t.Tile.Color, t.Tile.Form);

        private List<TileOnBoard> TilesOnBoardModelToEntity(List<TileOnBoardModel> tilesOnBoard)
        {
            var tiles = new List<TileOnBoard>();
            var tilesPersistence = DbContext.Tiles.Where(t => tilesOnBoard.Select(tb => tb.TileId).Contains(t.Id)).ToList();
            foreach (var tilePersistence in tilesPersistence)
            {
                var tileOnGame = tilesOnBoard.FirstOrDefault(tb => tb.TileId == tilePersistence.Id);
                tiles.Add(new TileOnBoard(tilePersistence.Id, tilePersistence.Color, tilePersistence.Form, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
            }
            return tiles;
        }


        private TileOnBag TileOnBagModelToEntity(TileOnBagModel tb)
           => new TileOnBag(tb.Id, tb.Tile.Color, tb.Tile.Form);

    }
}
