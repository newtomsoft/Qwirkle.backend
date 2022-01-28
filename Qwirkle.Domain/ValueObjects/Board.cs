namespace Qwirkle.Domain.ValueObjects;

public record Board(HashSet<TileOnBoard> Tiles)
{
    public static Board From(IEnumerable<TileOnBoard> tiles) => new(tiles.ToHashSet());
    public static Board From(Board board) => new(board.Tiles.Select(TileOnBoard.From).ToHashSet());
    public static Board Empty() => new(new HashSet<TileOnBoard>());

    public void AddTiles(IEnumerable<TileOnBoard> tiles) => Tiles.UnionWith(tiles);
    public bool IsIsolatedTile(TileOnBoard tile) => IsIsolated(Coordinates.From(tile.Coordinates.X, tile.Coordinates.Y));
    public bool IsFreeTile(TileOnBoard tile) => IsFree(tile.Coordinates);
    public List<Coordinates> GetFreeAdjoiningCoordinatesToTiles(Coordinates originCoordinates = null)
    {
        originCoordinates ??= Coordinates.From(0, 0);
        var coordinates = new List<Coordinates>();
        if (Tiles.Count == 0) return new List<Coordinates> { originCoordinates };
        for (var x = XMinToPlay(); x <= XMaxToPlay(); x++)
            for (var y = YMinToPlay(); y <= YMaxToPlay(); y++)
            {
                var coordinate = Coordinates.From(x, y);
                if (IsFree(coordinate) && IsIsolated(coordinate)) coordinates.Add(coordinate);
            }
        return coordinates;
    }

    private int XMinToPlay() => Tiles.Min(t => t.Coordinates.X) - 1;
    private int XMaxToPlay() => Tiles.Max(t => t.Coordinates.X) + 1;
    private int YMinToPlay() => Tiles.Min(t => t.Coordinates.Y) - 1;
    private int YMaxToPlay() => Tiles.Max(t => t.Coordinates.Y) + 1;
    private bool IsFree(Coordinates coordinate) => Tiles.All(t => t.Coordinates != coordinate);

    private bool IsIsolated(Coordinates coordinates)
    {
        var tileRight = Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Right());
        var tileLeft = Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Left());
        var tileTop = Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Top());
        var tileBottom = Tiles.FirstOrDefault(t => t.Coordinates == coordinates.Bottom());
        return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
    }
}