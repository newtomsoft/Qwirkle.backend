namespace Qwirkle.Domain.Services;

public static class ComputePoints
{
    public static int Compute(Game game, HashSet<TileOnBoard> tiles)
    {
        if (game.IsBoardEmpty() && tiles.Count == 1) return 1;

        var (_, coordinates) = tiles.First();
        if (tiles.Count(t => t.Coordinate.Y == coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinate.X == coordinates.X) != tiles.Count)
            return 0;

        var totalPoints = 0;
        int points;
        if (tiles.Count(t => t.Coordinate.Y == tiles.First().Coordinate.Y) == tiles.Count)
        {
            if ((points = ComputePointsInLine(tiles, game)) is 0) return 0;
            if (points is not 1) totalPoints += points;
            if (tiles.Count > 1)
            {
                foreach (var tile in tiles)
                {
                    if ((points = ComputePointsInColumn(new HashSet<TileOnBoard> { tile }, game)) is 0) return 0;
                    if (points is not 1) totalPoints += points;
                }
            }
        }

        if (tiles.Count(t => t.Coordinate.X == coordinates.X) != tiles.Count) return totalPoints;
        if ((points = ComputePointsInColumn(tiles, game)) == 0) return 0;
        if (points != 1) totalPoints += points;
        if (tiles.Count <= 1) return totalPoints;
        foreach (var tile in tiles)
        {
            if ((points = ComputePointsInLine(new HashSet<TileOnBoard> { tile }, game)) == 0) return 0;
            if (points != 1) totalPoints += points;
        }
        return totalPoints;
    }

    private static int ComputePointsInLine(IReadOnlyCollection<TileOnBoard> tiles, Game game)
    {
        var (_, (_, y)) = tiles.First();
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinate.X); var max = tiles.Max(t => t.Coordinate.X);
        var tilesBetweenReference = game.Board.Tiles.Where(t => t.Coordinate.Y == y && min <= t.Coordinate.X && t.Coordinate.X <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesRight = game.Board.Tiles.Where(t => t.Coordinate.Y == y && t.Coordinate.X >= max).OrderBy(t => t.Coordinate.X).ToList();
        var tilesRightConsecutive = tilesRight.FirstConsecutive(Direction.Right, max);
        allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

        var tilesLeft = game.Board.Tiles.Where(t => t.Coordinate.Y == y && t.Coordinate.X <= min).OrderByDescending(t => t.Coordinate.X).ToList();
        var tilesLeftConsecutive = tilesLeft.FirstConsecutive(Direction.Left, min);
        allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinate.X).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != CoreService.TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : CoreService.PointsForAQwirkle;
    }

    private static int ComputePointsInColumn(IReadOnlyCollection<TileOnBoard> tiles, Game game)
    {
        var (_, (x, _)) = tiles.First();
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinate.Y); var max = tiles.Max(t => t.Coordinate.Y);
        var tilesBetweenReference = game.Board.Tiles.Where(t => t.Coordinate.X == x && min <= t.Coordinate.Y && t.Coordinate.Y <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesUp = game.Board.Tiles.Where(t => t.Coordinate.X == x && t.Coordinate.Y >= max).OrderBy(t => t.Coordinate.Y).ToList();
        var tilesUpConsecutive = tilesUp.FirstConsecutive(Direction.Top, max);
        allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

        var tilesBottom = game.Board.Tiles.Where(t => t.Coordinate.X == x && t.Coordinate.Y <= min).OrderByDescending(t => t.Coordinate.Y).ToList();
        var tilesBottomConsecutive = tilesBottom.FirstConsecutive(Direction.Bottom, min);
        allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinate.Y).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != CoreService.TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : CoreService.PointsForAQwirkle;
    }

    private static bool AreNumbersConsecutive(IReadOnlyCollection<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

}