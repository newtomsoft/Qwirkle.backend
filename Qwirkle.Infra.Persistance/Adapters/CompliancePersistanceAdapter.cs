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

        public void UpdatePlayerPoints(Board board, Player player)
        {
            DbContext.GamePlayers.Update(PlayerEntitiesToGamePlayerPersistance(player));
            DbContext.SaveChanges();
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
            DbContext.Games.Update(new GamePersistance { Id = gameId, LastPlayedDate = DateTime.Now });

            var tilesOnBoard = DbContext.TilesOnBoard.Where(t => t.GameId == gameId).ToList();
            tilesOnBoard.AddRange(TileEntitiesToTilesOnBoardPersistance(gameId, tiles));

            DbContext.SaveChanges();
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
            var gamePlayerPersistance = DbContext.GamePlayers.Where(gp => gp.Id == player.Id).FirstOrDefault();
            gamePlayerPersistance.Points = player.Points;
            return gamePlayerPersistance;
        }
    }
}
