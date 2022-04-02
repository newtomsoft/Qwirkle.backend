namespace Qwirkle.Domain.ValueObjects;

public record Board(HashSet<TileOnBoard> Tiles)
{
    public static Board From(IEnumerable<TileOnBoard> tiles) => new(tiles.ToHashSet());
    public static Board From(Board board) => new(board.Tiles.Select(TileOnBoard.From).ToHashSet());
    public static Board Empty => new(new HashSet<TileOnBoard>());

    public void AddTiles(IEnumerable<TileOnBoard> tiles) => Tiles.UnionWith(tiles);
    public bool IsIsolatedTile(TileOnBoard tile) => IsIsolated(Coordinate.From(tile.Coordinate.X, tile.Coordinate.Y));
    public bool IsFreeTile(TileOnBoard tile) => IsFree(tile.Coordinate);
    public List<Coordinate> GetFreeAdjoiningCoordinatesToTiles(Coordinate originCoordinate = null)
    {
        originCoordinate ??= Coordinate.From(0, 0);
        var coordinates = new List<Coordinate>();
        if (Tiles.Count == 0) return new List<Coordinate> { originCoordinate };
        for (var x = XMinToPlay(); x <= XMaxToPlay(); x++)
            for (var y = YMinToPlay(); y <= YMaxToPlay(); y++)
            {
                var coordinate = Coordinate.From(x, y);
                if (IsFree(coordinate) && IsIsolated(coordinate)) coordinates.Add(coordinate);
            }
        return coordinates;
    }

    private int XMinToPlay() => Tiles.Min(t => t.Coordinate.X) - 1;
    private int XMaxToPlay() => Tiles.Max(t => t.Coordinate.X) + 1;
    private int YMinToPlay() => Tiles.Min(t => t.Coordinate.Y) - 1;
    private int YMaxToPlay() => Tiles.Max(t => t.Coordinate.Y) + 1;
    private bool IsFree(Coordinate coordinate) => Tiles.All(t => t.Coordinate != coordinate);

    private bool IsIsolated(Coordinate coordinate)
    {
        var tileRight = Tiles.FirstOrDefault(t => t.Coordinate == coordinate.Right());
        var tileLeft = Tiles.FirstOrDefault(t => t.Coordinate == coordinate.Left());
        var tileTop = Tiles.FirstOrDefault(t => t.Coordinate == coordinate.Top());
        var tileBottom = Tiles.FirstOrDefault(t => t.Coordinate == coordinate.Bottom());
        return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
    }
}