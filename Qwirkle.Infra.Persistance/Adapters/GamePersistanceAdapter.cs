using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.GameContext.Entities;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class GamePersistanceAdapter : IGamePersistance
    {
        private DefaultDbContext DbContext { get; }


        public GamePersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        
        public Game GetGame(int gameId)
        {
            var gamePersistance = DbContext.Games.Where(g => g.Id == gameId).FirstOrDefault();
            var tilesOnGamePersistance = DbContext.TilesOnGame.Where(tb => tb.GameId == gameId).ToList();
            var tiles = TilesOnGamePersistanceToTiles(tilesOnGamePersistance);
            var tilesOnBag = DbContext.TilesOnBag.Where(g => g.GameId == gameId).Include(tb => tb.Tile).ToList();
            Bag bag = new Bag { Id = gameId, Tiles = new List<Tile>() };
            tilesOnBag.ForEach(t => bag.Tiles.Add(TileOnBagToTile(t)));
            return new Game(gamePersistance.Id, tiles, bag);
        }

        private List<Tile> TilesOnGamePersistanceToTiles(List<TileOnGamePersistance> tilesOnGame)
        {
            var tiles = new List<Tile>();
            var tilesPersistance = DbContext.Tiles.Where(t => tilesOnGame.Select(tb => tb.TileId).Contains(t.Id)).ToList();
            foreach (var tilePersistance in tilesPersistance)
            {
                var tileOnGame = tilesOnGame.FirstOrDefault(tb => tb.TileId == tilePersistance.Id);
                tiles.Add(new Tile(tilePersistance.Id, tilePersistance.Color, tilePersistance.Form, new CoordinatesInGame(tileOnGame.PositionX, tileOnGame.PositionY)));
            }
            return tiles;
        }


         private Tile TileOnBagToTile(TileOnBagPersistance tb)
            => new Tile(tb.Id, tb.Tile.Color, tb.Tile.Form, new CoordinatesInGame());

    }
}
