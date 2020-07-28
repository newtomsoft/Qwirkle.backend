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
            int points;
            if ((points = CanTilesBePlayed(board, tiles)) == 0) return false;

            // todo
            //Player points
            // et persistance.Player poins

            board.Tiles.AddRange(tiles);
            Persistance.UpdateBoard(board.Id, tiles);



            //Persistance.RemovePlayerTiles(tiles);
            return true;
        }


        public int CanTilesBePlayed(Board board, List<Tile> tiles)
        {
            if (board.Tiles.Count == 0) return tiles.Count;
            //TODO !!!!! tester si tiles pas isolés !

            bool AreAllTilesIsolated = true;
            foreach (var tile in tiles)
                if (AreTileIsolated(board, tile))
                    AreAllTilesIsolated = false;
            if (AreAllTilesIsolated) return 0;

            int totalPoints;
            if ((totalPoints = CountTilesMakedValidRow(board, tiles)) == 0) return 0;
            return totalPoints;
            //todo : tester si qwarkle et +6 points
        }

        private bool AreTilesMakeValidRow(Board board, List<Tile> tiles)
        {
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
                return false;

            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
            {
                if (!AreTilesMakeValidLine(board, tiles)) return false;

                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if (!AreTilesMakeValidColumn(board, new List<Tile> { tile })) return false;
                    }
                }
            }
            if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) == tiles.Count)
            {
                if (!AreTilesMakeValidColumn(board, tiles)) return false;

                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if (!AreTilesMakeValidLine(board, new List<Tile> { tile })) return false;
                    }
                }
            }
            return true;
        }

        public int CountTilesMakedValidRow(Board board, List<Tile> tiles)
        {
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
                return 0;
            
            int tatalPoints = 0;
            int points = 0;
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
            {
                if ((points = CountTilesMakedValidLine(board, tiles)) == 0) return 0;
                if (points != 1) tatalPoints += points;
                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if ((points = CountTilesMakedValidColumn(board, new List<Tile> { tile })) == 0) return 0;
                        if (points != 1) tatalPoints += points ;
                    }
                }
            }
            if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) == tiles.Count)
            {
                if ((points = CountTilesMakedValidColumn(board, tiles)) == 0) return 0;
                if (points != 1) tatalPoints += points;

                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if ((points = CountTilesMakedValidLine(board, new List<Tile> { tile })) == 0) return 0;
                        if (points != 1) tatalPoints += points;
                    }
                }
            }
            return tatalPoints;
        }

        private bool AreTilesMakeValidLine(Board board, List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
            var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesRight = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
            var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
            allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

            var tilesLeft = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
            var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
            allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return false;

            return true;
        }

        private bool AreTilesMakeValidColumn(Board board, List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
            var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesUp = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
            var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
            allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

            var tilesBottom = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
            var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
            allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return false;

            return true;
        }

        public int CountTilesMakedValidLine(Board board, List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
            var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesRight = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
            var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
            allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

            var tilesLeft = board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
            var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
            allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return allTilesAlongReferenceTiles.Count;
        }

        public int CountTilesMakedValidColumn(Board board, List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
            var tilesBetweenReference = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesUp = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
            var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
            allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

            var tilesBottom = board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
            var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
            allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return allTilesAlongReferenceTiles.Count;
        }

        private static bool AreNumbersConsecutive(List<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

        private bool AreTileIsolated(Board board, Tile tile)
        {
            var tileRight = board.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Right());
            var tileLeft = board.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Left());
            var tileTop = board.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Top());
            var tileBottom = board.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Bottom());
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
