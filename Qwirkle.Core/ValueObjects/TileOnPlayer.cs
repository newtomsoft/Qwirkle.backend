namespace Qwirkle.Core.ValueObjects;

public record TileOnPlayer(RackPosition RackPosition, int Id, TileColor Color, TileShape Shape) : Tile(Id, Color, Shape)
{
    public TileOnBoard ToTileOnBoard(Coordinates coordinates) => TileOnBoard.From(this, coordinates);
}
