namespace Qwirkle.Domain.ValueObjects;

public record TileOnBag(TileColor Color, TileShape Shape) : Tile(Color, Shape);