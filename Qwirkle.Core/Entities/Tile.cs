namespace Qwirkle.Core.Entities;

public class Tile
{
    public int Id { get; }
    public TileColor Color { get; }
    public TileShape Shape { get; }

    protected Tile(Tile tile)
    {
        Id = tile.Id;
        Color = tile.Color;
        Shape = tile.Shape;
    }

    public Tile(int id, TileColor color, TileShape shape)
    {
        Id = id;
        Color = color;
        Shape = shape;
    }

    public Tile(TileColor color, TileShape shape) : this(0, color, shape)
    { }

    public Tile(int id)
    {
        Id = id;
    }

    public bool OnlyShapeOrColorEqual(Tile tile)
    {
        if (Color == tile.Color && Shape != tile.Shape || Color != tile.Color && Shape == tile.Shape)
            return true;
        return false;
    }
}
