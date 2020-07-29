using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.BagContext.Entities;
using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class BagPersistanceAdapter : IBagPersistance
    {
        private DefaultDbContext DbContext { get; }


        public BagPersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        public List<Tile> GetAllTilesOfBag(int gameId)
        {
            List<Tile> tilesEntities = new List<Tile>();
            DbContext.TilesOnBag.Where(t => t.GameId == gameId).ForEachAsync(t => tilesEntities.Add(TileModelToTileEntity(t))).Wait();
            return tilesEntities;
        }

        public Tile GetRandomTileOfBag(int gameId) => TileModelToTileEntity(DbContext.TilesOnBag.Where(t => t.GameId == gameId).OrderBy(_ => Guid.NewGuid()).FirstOrDefault());

        public int CountAllTilesOfBag(int gameId) => DbContext.TilesOnBag.Where(t => t.GameId == gameId).Count();

        public void SaveTile(Tile tile)
        {
            DbContext.TilesOnBag.Add(TileEntityToTileOnBag(tile));
            DbContext.SaveChanges();
        }

        public void DeleteAllTilesOfBag(int gameId)
        {
            DbContext.TilesOnBag.RemoveRange(DbContext.TilesOnBag.Where(t => t.GameId == gameId));
            DbContext.SaveChanges();
        }

        private Tile TileModelToTileEntity(TileOnBagPersistance tile) => new Tile(tile.Id, tile.GameId, tile.Color, tile.Form);
        private TileOnBagPersistance TileEntityToTileOnBag(Tile tile) => new TileOnBagPersistance { Id = tile.Id, GameId = tile.GameId, Color = tile.Color, Form = tile.Form };

       
    }
}
