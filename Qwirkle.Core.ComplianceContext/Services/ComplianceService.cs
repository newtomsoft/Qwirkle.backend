using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Services
{
    public class ComplianceService : IRequestComplianceService
    {
        private ICompliancePersistance Persistance { get; }

        public ComplianceService(ICompliancePersistance persistance) => Persistance = persistance;

        public bool IsTileCanBePlayedAtThisCoordinate(Board board, Tile tile)
        {
            if (IsBoardIsEmpty(board)) return true;
            if (IsCoordinateOnBoardIsTaken(board, tile.Coordinates)) return false;
            if (!IsTheyAreTilesAroundCoordinates(board, tile.Coordinates)) return false;
            if (ColumnAndLineMadeByTileRespectsRules(board, tile)) return true;

            return false;
        }

        private bool ColumnAndLineMadeByTileRespectsRules(Board board, Tile tile)
        {
            foreach (Direction direction in (Direction[])Enum.GetValues(typeof(Direction)))
            {
                CoordinatesInBoard currentCoordinates = tile.Coordinates;
                while (true)
                {
                    if (direction == Direction.Right) currentCoordinates = currentCoordinates.Right();
                    else if (direction == Direction.Bottom) currentCoordinates = currentCoordinates.Bottom();
                    else if (direction == Direction.Left) currentCoordinates = currentCoordinates.Left();
                    else if (direction == Direction.Top) currentCoordinates = currentCoordinates.Top();

                    var currentTile = board.Tiles.FirstOrDefault(t => t.Coordinates == currentCoordinates);
                    if (currentTile == null)
                        break;
                    if (currentTile.Color == tile.Color && currentTile.Form == tile.Form || currentTile.Color != tile.Color && currentTile.Form != tile.Form)
                        return false;
                }
            }
            return true;
        }

        private bool IsBoardIsEmpty(Board board)
        {
            return board.Tiles.Count == 0;
        }

        private bool IsCoordinateOnBoardIsTaken(Board board, CoordinatesInBoard coordinates)
        {
            var tile = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates);
            return tile != null;
        }

        private bool IsTheyAreTilesAroundCoordinates(Board board, CoordinatesInBoard coordinates)
        {
            var tileRight = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Right());
            var tileLeft = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Left());
            var tileTop = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Top());
            var tileBottom = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Bottom());
            return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
        }

        public bool IsPlayerTurn(int gameId, int playerId) //todo private
        {
            return Persistance.IsPlayerTurn(gameId, playerId);
        }

        public void SetPlayerTurn(int gameId, int playerId) //todo private
        {
            Persistance.SetPlayerTurn(gameId, playerId);
        }
    }
}
