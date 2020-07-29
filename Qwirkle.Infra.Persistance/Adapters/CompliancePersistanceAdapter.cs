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
        private readonly DefaultDbContext _dbContext;

        public CompliancePersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            _dbContext = defaultDbContext;
        }

        public void UpdatePlayerPoints(Board board, Player player)
        {
            _dbContext.GamePlayers.Update(PlayerEntitiesToGamePlayerPersistance(player));
            _dbContext.SaveChanges();
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
            _dbContext.Games.Update(new GamePersistance { Id = gameId, LastPlayedDate = DateTime.Now });

            var tilesOnBoard = _dbContext.TilesOnBoard.Where(t => t.GameId == gameId).ToList();
            tilesOnBoard.AddRange(TileEntitiesToTilesOnBoardPersistance(gameId, tiles));

            _dbContext.SaveChanges();
        }

        private IEnumerable<TileOnBoardPersistance> TileEntitiesToTilesOnBoardPersistance(int gameId, List<Tile> tiles)
        {
            var tilesOnBoard = new List<TileOnBoardPersistance>();
            foreach (var tile in tiles)
            {
                tilesOnBoard.Add(new TileOnBoardPersistance { Id = tile.Id, GameId = gameId, Color = tile.Color, Form = tile.Form, PositionX = tile.Coordinates.X, PositionY = tile.Coordinates.Y });
            }
            return tilesOnBoard;
        }

        private GamePlayerPersistance PlayerEntitiesToGamePlayerPersistance(Player player)
        {
            var gamePlayerPersistance = _dbContext.GamePlayers.Where(gp => gp.Id == player.Id).FirstOrDefault();
            gamePlayerPersistance.Points = player.Points;
            return gamePlayerPersistance;
        }
    }
}
