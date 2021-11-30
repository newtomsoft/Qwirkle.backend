namespace Qwirkle.Core.ValueObjects;

public record Tile(TileColor Color, TileShape Shape)
{
    protected Tile(Tile tile) => (Color, Shape) = tile;
    
    public bool OnlyShapeOrColorEqual(Tile tile) => Color == tile.Color && Shape != tile.Shape || Color != tile.Color && Shape == tile.Shape;
}
