namespace Qwirkle.Domain.ValueObjects;

[Serializable]
public record TileOnRack(RackPosition RackPosition, TileColor Color, TileShape Shape) : Tile(Color, Shape)
{
    public TileOnRack(RackPosition rackPosition, Tile tile) : this(rackPosition, tile.Color, tile.Shape) { }

    public TileOnRack() : this(0, TileColor.Blue, TileShape.Circle) { }
    public Tile ToTile() => new(Color, Shape);
    public TileOnRack ToHiddenTile() => new(RackPosition, 0, 0);
}