namespace Qwirkle.Domain.ValueObjects;

public record TileOnPlayer(RackPosition RackPosition, TileColor Color, TileShape Shape) : Tile(Color, Shape)
{
    public TileOnBoard ToTileOnBoard(Coordinates coordinates) => TileOnBoard.From(this, coordinates);
    public Tile ToTile() => new(Color, Shape);
}
