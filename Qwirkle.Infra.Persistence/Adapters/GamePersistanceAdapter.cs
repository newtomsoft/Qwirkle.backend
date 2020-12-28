using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.GameContext.Entities;
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
            var tilesOnGame = DbContext.TilesOnGame.Where(tb => tb.GameId == gameId).ToList();
            var tiles = TilesOnGamePersistenceToTiles(tilesOnGame);
            var tilesOnBag = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
            Bag bag = new Bag { Id = gameId, Tiles = new List<Tile>() };
            tilesOnBag.ForEach(t => bag.Tiles.Add(TileOnBagToTile(t)));
            return new Game(game.Id, tiles, bag);
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


        private Tile TileOnBagToTile(TileOnBagPersistence tb)
           => new Tile(tb.Id, tb.Tile.Color, tb.Tile.Form, new CoordinatesInGame());

    }
}
