namespace Qwirkle.Core.UseCases;

public class ComplianceUseCase
{
    private const int TilesNumberForAQwirkle = 6;
    private const int PointsForAQwirkle = 12;

    public Game Game { get; set; }

    public int ComputePoints(Game game, List<TileOnBoard> tiles)
    {
        Game = game;

        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
            return 0;

        var totalPoints = 0;
        int points;
        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
        {
            if ((points = ComputePointsInLine(tiles)) is 0) return 0;
            if (points is not 1) totalPoints += points;
            if (tiles.Count > 1)
            {
                foreach (var tile in tiles)
                {
                    if ((points = ComputePointsInColumn(new List<TileOnBoard> { tile })) is 0) return 0;
                    if (points is not 1) totalPoints += points;
                }
            }
        }

        if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count) return totalPoints;
        if ((points = ComputePointsInColumn(tiles)) == 0) return 0;
        if (points != 1) totalPoints += points;
        if (tiles.Count <= 1) return totalPoints;
        foreach (var tile in tiles)
        {
            if ((points = ComputePointsInLine(new List<TileOnBoard> { tile })) == 0) return 0;
            if (points != 1) totalPoints += points;
        }
        return totalPoints;
    }

    private int ComputePointsInLine(IReadOnlyList<TileOnBoard> tiles)
    {
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
        var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesRight = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
        var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
        allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

        var tilesLeft = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
        var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
        allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private int ComputePointsInColumn(IReadOnlyList<TileOnBoard> tiles)
    {
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
        var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesUp = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
        var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
        allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

        var tilesBottom = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
        var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
        allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private static bool AreNumbersConsecutive(IReadOnlyCollection<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

}