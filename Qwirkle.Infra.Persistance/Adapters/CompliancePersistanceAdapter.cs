using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.BagContext.Entities;
using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class CompliancePersistanceAdapter : ICompliancePersistance
    {
        private DefaultDbContext _dbContext { get; }


        public CompliancePersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            _dbContext = defaultDbContext;
        }

        public bool IsPlayerTurn(int gameId, int playerId)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerTurn(int gameId, int playerId)
        {
            throw new NotImplementedException();
        }


        //pour inspiration
        //public Tile GetRandomTileOfBag(int gameId) => TileModelToTileEntity(_dbContext.TilesOnBag.Where(t => t.GameId == gameId).OrderBy(_ => Guid.NewGuid()).FirstOrDefault());
        //public int CountAllTilesOfBag(int gameId) => _dbContext.TilesOnBag.Where(t => t.GameId == gameId).Count();
        //private Tile TileModelToTileEntity(TileOnBag tile) => new Tile(tile.Id, tile.GameId, tile.Color, tile.Form);
        //private TileOnBag TileEntityToTileOnBag(Tile tile) => new TileOnBag { Id = tile.Id, GameId = tile.GameId, Color = tile.Color, Form = tile.Form };


    }
}
