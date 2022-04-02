namespace Qwirkle.Domain.ValueObjects;

public record Tile(TileColor Color, TileShape Shape) : IComparable
{
    public Tile() : this(TileColor.Blue, TileShape.Circle) { }

    protected Tile(Tile tile) => (Color, Shape) = tile;

    public bool OnlyShapeOrColorEqual(Tile tile) => Color == tile.Color && Shape != tile.Shape || Color != tile.Color && Shape == tile.Shape;

    public TileOnRack ToTileOnRack(RackPosition rackPosition) => new(rackPosition, this);

    public int CompareTo(object obj) => obj is not Tile(var color, var shape) ? 1 : Shape != shape ? Shape.CompareTo(shape) : Color.CompareTo(color);

    public override string ToString() => $"{Shape}-{Color}";
}