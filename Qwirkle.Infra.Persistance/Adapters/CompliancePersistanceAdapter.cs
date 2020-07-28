using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Tile = Qwirkle.Core.ComplianceContext.Entities.Tile;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class CompliancePersistanceAdapter : ICompliancePersistance
    {
        private readonly DefaultDbContext _dbContext;

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

        public void UpdateBoard(int gameId, List<Tile> tiles)
        {
            _dbContext.Games.Update(new Game { Id = gameId, LastPlayedDate = DateTime.Now });

            var tilesOnBoard = _dbContext.TilesOnBoard.Where(t => t.GameId == gameId).ToList();
            tilesOnBoard.AddRange(TileEntitiesToTilesOnBoard(gameId, tiles));

            _dbContext.SaveChanges();
        }

        private IEnumerable<TileOnBoard> TileEntitiesToTilesOnBoard(int gameId, List<Tile> tiles)
        {
            var tilesOnBoard = new List<TileOnBoard>();
            foreach (var tile in tiles)
            {
                tilesOnBoard.Add(new TileOnBoard { Id = tile.Id, GameId = gameId, Color = tile.Color, Form = tile.Form, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y });
            }
            return tilesOnBoard;
        }


        //pour inspiration
        //public Tile GetRandomTileOfBag(int gameId) => TileModelToTileEntity(_dbContext.TilesOnBag.Where(t => t.GameId == gameId).OrderBy(_ => Guid.NewGuid()).FirstOrDefault());
        //public int CountAllTilesOfBag(int gameId) => _dbContext.TilesOnBag.Where(t => t.GameId == gameId).Count();
        //private Tile TileModelToTileEntity(TileOnBag tile) => new Tile(tile.Id, tile.GameId, tile.Color, tile.Form);
        //private TileOnBag TileEntityToTileOnBag(Tile tile) => new TileOnBag { Id = tile.Id, GameId = tile.GameId, Color = tile.Color, Form = tile.Form };


    }
}
