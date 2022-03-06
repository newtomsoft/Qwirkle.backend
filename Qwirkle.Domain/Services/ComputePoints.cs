namespace Qwirkle.Domain.Services;

public static class ComputePoints
{
    public static int Compute(Game game, HashSet<TileOnBoard> tiles)
    {
        if (game.IsBoardEmpty() && tiles.Count == 1) return 1;

        var (_, coordinates) = tiles.First();
        if (tiles.Count(t => t.Coordinates.Y == coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == coordinates.X) != tiles.Count)
            return 0;

        var totalPoints = 0;
        int points;
        if (tiles.Count(t => t.Coordinates.Y == tiles.First().Coordinates.Y) == tiles.Count)
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

        if (tiles.Count(t => t.Coordinates.X == coordinates.X) != tiles.Count) return totalPoints;
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
        var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
        var tilesBetweenReference = game.Board.Tiles.Where(t => t.Coordinates.Y == y && min <= t.Coordinates.X && t.Coordinates.X <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesRight = game.Board.Tiles.Where(t => t.Coordinates.Y == y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
        var tilesRightConsecutive = tilesRight.FirstConsecutive(Direction.Right, max);
        allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

        var tilesLeft = game.Board.Tiles.Where(t => t.Coordinates.Y == y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
        var tilesLeftConsecutive = tilesLeft.FirstConsecutive(Direction.Left, min);
        allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != CoreService.TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : CoreService.PointsForAQwirkle;
    }

    private static int ComputePointsInColumn(IReadOnlyCollection<TileOnBoard> tiles, Game game)
    {
        var (_, (x, _)) = tiles.First();
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
        var tilesBetweenReference = game.Board.Tiles.Where(t => t.Coordinates.X == x && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesUp = game.Board.Tiles.Where(t => t.Coordinates.X == x && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
        var tilesUpConsecutive = tilesUp.FirstConsecutive(Direction.Top, max);
        allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

        var tilesBottom = game.Board.Tiles.Where(t => t.Coordinates.X == x && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
        var tilesBottomConsecutive = tilesBottom.FirstConsecutive(Direction.Bottom, min);
        allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != CoreService.TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : CoreService.PointsForAQwirkle;
    }

    private static bool AreNumbersConsecutive(IReadOnlyCollection<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

}