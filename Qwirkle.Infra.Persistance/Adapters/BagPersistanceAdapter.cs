using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.BagContext.Entities;
using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Core.CommonContext;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class BagPersistanceAdapter : IBagPersistance
    {
        private DefaultDbContext _dbContext { get; }


        public BagPersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            _dbContext = defaultDbContext;
        }

        public List<Tile> GetAllTilesOfBag(int gameId)
        {
            List<Tile> tilesEntities = new List<Tile>();
            _dbContext.TilesOnBag.Where(t => t.GameId == gameId).ForEachAsync(t => tilesEntities.Add(TileModelToTileEntity(t))).Wait();
            return tilesEntities;
        }

        public Tile GetRandomTileOfBag(int gameId) => TileModelToTileEntity(_dbContext.TilesOnBag.Where(t => t.GameId == gameId).OrderBy(_ => Guid.NewGuid()).FirstOrDefault());

        public int CountAllTilesOfBag(int gameId) => _dbContext.TilesOnBag.Where(t => t.GameId == gameId).Count();

        public void SaveTile(Tile tile)
        {
            _dbContext.TilesOnBag.Add(TileEntityToTileOnBag(tile));
            _dbContext.SaveChanges();
        }

        public void DeleteAllTilesOfBag(int gameId)
        {
            _dbContext.TilesOnBag.RemoveRange(_dbContext.TilesOnBag.Where(t => t.GameId == gameId));
            _dbContext.SaveChanges();
        }

        private Tile TileModelToTileEntity(TileOnBagPersistance tile) => new Tile(tile.Id, tile.GameId, TileColor.Blue, TileForm.Ring); //todo !
        private TileOnBagPersistance TileEntityToTileOnBag(Tile tile) => new TileOnBagPersistance { Id = tile.Id, GameId = tile.GameId};

       
    }
}
