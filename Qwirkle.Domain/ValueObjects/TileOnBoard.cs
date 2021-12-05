namespace Qwirkle.Domain.ValueObjects;

public record TileOnBoard(Tile Tile, Coordinates Coordinates) : Tile(Tile)
{
    public TileOnBoard(TileColor color, TileShape shape, Coordinates coordinates) : this(new Tile(color, shape), coordinates) { }
    public static TileOnBoard From(TileOnPlayer tile, Coordinates coordinates) => new(tile, coordinates);
    public static TileOnBoard From(Tile tile, Coordinates coordinates) => new(tile, coordinates);

    private TileOnBoard(TileOnPlayer tile, Coordinates coordinates) : this(tile.Color, tile.Shape, coordinates) { }

    public Tile ToTile() => new(Color, Shape);
}
