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

        public bool PlayTiles(Board board, List<Tile> tiles)
        {
            if (!CanTilesBePlayed(board, tiles)) return false;

            board.Tiles.AddRange(tiles);
            Persistance.UpdateBoard(board.Id, tiles);

            //Persistance.RemovePlayerTiles(tiles);
            return true;
        }

        public bool CanTilesBePlayed(Board board, List<Tile> tiles)
        {
            if (!AreTilesMakeValideRow(board, tiles)) return false;
            if (!CanEachTileBePlayed(board, tiles)) return false;
            return true;
        }
        public bool CanTileBePlayed(Board board, Tile tile)
        {
            if (IsBoardEmpty(board)) return true;
            if (AreCoordinateOnBoardTaken(board, tile.Coordinates)) return false;
            if (!AreTilesAroundCoordinates(board, tile.Coordinates)) return false;
            if (AreColumnAndLineByTileRespectsRules(board, tile)) return true;
            return false;
        }

        private bool AreTilesMakeValideRow(Board board, List<Tile> tiles)
        {
            
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
                return false;

            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
            {
                var allTilesAlongReferenceTiles = tiles.ToList();
                var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
                var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min < t.Coordinates.X && t.Coordinates.X < max);
                allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

                var tilesRight = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
                var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
                allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

                var tilesLeft = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
                var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
                allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);
                if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()))
                    return false;
                if (!allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                    return false;
            }
            if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) == tiles.Count)
            {
                var allTilesAlongReferenceTiles = tiles.ToList();
                var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
                var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min < t.Coordinates.Y && t.Coordinates.Y < max);
                allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

                var tilesUp = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
                var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
                allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

                var tilesBottom = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
                var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
                allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);
                if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()))
                    return false;
                if (!allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                    return false;
            }

            return true;


            return AreNumbersConsecutive(GetAllTilesCoordinatesBetweenReferenceTiles(board.Tiles, tiles));

        }

        private static List<sbyte> GetAllTilesCoordinatesBetweenReferenceTiles(List<Tile> tiles, List<Tile> referenceTiles)
        {
            if (referenceTiles.Count(t => t.Coordinates.Y == referenceTiles[0].Coordinates.Y) == referenceTiles.Count)
            {
                var min = referenceTiles.Min(t => t.Coordinates.X); var max = referenceTiles.Max(t => t.Coordinates.X);
                var tilesBetweenReference = tiles.Where(t => t.Coordinates.Y == referenceTiles[0].Coordinates.Y && min < t.Coordinates.X && t.Coordinates.X < max);
                var referenceTilesX = referenceTiles.Select(t => t.Coordinates.X).ToList();
                referenceTilesX.AddRange(tilesBetweenReference.Select(t => t.Coordinates.X));
                return referenceTilesX;
            }
            else if (referenceTiles.Count(t => t.Coordinates.X == referenceTiles[0].Coordinates.X) == referenceTiles.Count)
            {
                var min = referenceTiles.Min(t => t.Coordinates.Y); var max = referenceTiles.Max(t => t.Coordinates.Y);
                var tilesBetweenReference = tiles.Where(t => t.Coordinates.X == referenceTiles[0].Coordinates.X && min < t.Coordinates.Y && t.Coordinates.Y < max);
                var referenceTilesY = referenceTiles.Select(t => t.Coordinates.Y).ToList();
                referenceTilesY.AddRange(tilesBetweenReference.Select(t => t.Coordinates.Y));
                return referenceTilesY;
            }
            else
                return new List<sbyte>();
        }

        private static bool AreNumbersConsecutive(List<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

        private bool CanEachTileBePlayed(Board board, List<Tile> tiles)
        {
            Board boardCopy = new Board(board);
            var tileToTest = tiles;
            while (tileToTest.Count > 0)
            {
                var tilesNotTested = new List<Tile>();
                foreach (var tile in tileToTest)
                {
                    if (CanTileBePlayed(boardCopy, tile))
                        boardCopy.Tiles.Add(tile);
                    else
                        tilesNotTested.Add(tile);
                }
                if (tilesNotTested.Count == tileToTest.Count)
                    return false;
                if (tilesNotTested.Count == 0)
                    return true;
                tileToTest = tilesNotTested;
            }
            return false;
        }

        private bool IsBoardEmpty(Board board)
        {
            return board.Tiles.Count == 0;
        }

        private bool AreCoordinateOnBoardTaken(Board board, CoordinatesInBoard coordinates)
        {
            var tile = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates);
            return tile != null;
        }

        private bool AreTilesAroundCoordinates(Board board, CoordinatesInBoard coordinates)
        {
            var tileRight = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Right());
            var tileLeft = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Left());
            var tileTop = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Top());
            var tileBottom = board.Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Bottom());
            return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
        }
        private bool AreColumnAndLineByTileRespectsRules(Board board, Tile tile)
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

        public bool IsPlayerTurn(int gameId, int playerId) //todo private
        {
            return Persistance.IsPlayerTurn(gameId, playerId);
        }

        public void SetPlayerTurn(int gameId, int playerId) //todo private
        {
            Persistance.SetPlayerTurn(gameId, playerId);
        }
    }
    public static class TileExtension
    {
        public static List<Tile> FirstConsecutives(this List<Tile> tiles, Direction direction, sbyte reference)
        {
            int diff = direction == Direction.Right || direction == Direction.Top ? -1 : 1;
            var result = new List<Tile>();
            if (tiles.Count == 0) return result;
            if ((direction == Direction.Left || direction == Direction.Right) && reference != tiles[0].Coordinates.X + diff) return result;
            if ((direction == Direction.Top || direction == Direction.Bottom) && reference != tiles[0].Coordinates.Y + diff) return result;

            result.Add(tiles[0]);
            for (int i = 1; i < tiles.Count; i++)
            {
                if ((direction == Direction.Left || direction == Direction.Right) && tiles[i - 1].Coordinates.X == tiles[i].Coordinates.X + diff && tiles[i - 1].Coordinates.Y == tiles[i].Coordinates.Y
                 || (direction == Direction.Top || direction == Direction.Bottom) && tiles[i - 1].Coordinates.Y == tiles[i].Coordinates.Y + diff && tiles[i - 1].Coordinates.X == tiles[i].Coordinates.X)
                    result.Add(tiles[i]);
                else
                    break;
            }
            return result;
        }

        public static bool AreRowByTileRespectsRules(this List<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
                for (int j = i + 1; j < tiles.Count; j++)
                    if (!tiles[i].HaveFormOrColorOnlyEqual(tiles[j])) return false;

            return true;
        }

    }
}
