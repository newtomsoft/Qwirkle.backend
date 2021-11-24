namespace Qwirkle.Core.Entities;

public class Board
{
    public List<TileOnBoard> Tiles { get; set; }

    public Board(List<TileOnBoard> tiles)
    {
        Tiles = tiles;
    }

    public bool IsTileIsolated(TileOnBoard tile)
    {
        //IsIsolated(tile.Coordinates);
        var tileRight = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Right());
        var tileLeft = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Left());
        var tileTop = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Top());
        var tileBottom = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Bottom());
        return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
    }

    public List<Coordinates> GetPossibleCoordinatesToPlay()
    {
        var coordinates = new List<Coordinates>();
        if (XMaxToPlay() == int.MaxValue) return new List<Coordinates> { Coordinates.From(0, 0) };
        for (var x = XMinToPlay(); x <= XMaxToPlay(); x++)
            for (var y = YMinToPlay(); y <= YMaxToPlay(); y++)
            {
                var coordinate = new Coordinates((sbyte)x, (sbyte)y);
                if (IsFree(coordinate) & IsIsolated(coordinate)) coordinates.Add(coordinate);
            }
        return coordinates;
    }

    private int XMinToPlay() => Tiles.Count == 0 ? int.MaxValue : Tiles.Min(t => t.Coordinates.X) - 1;
    private int XMaxToPlay() => Tiles.Count == 0 ? int.MaxValue : Tiles.Max(t => t.Coordinates.X) + 1;
    private int YMinToPlay() => Tiles.Count == 0 ? int.MaxValue : Tiles.Min(t => t.Coordinates.Y) - 1;
    private int YMaxToPlay() => Tiles.Count == 0 ? int.MaxValue : Tiles.Max(t => t.Coordinates.Y) + 1;
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
