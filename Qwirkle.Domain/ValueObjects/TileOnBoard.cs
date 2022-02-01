namespace Qwirkle.Domain.ValueObjects;

public record TileOnBoard(Tile Tile, Coordinates Coordinates) : Tile(Tile)
{
    public TileOnBoard(TileColor color, TileShape shape, Coordinates coordinates) : this(new Tile(color, shape), coordinates) { }
    public static TileOnBoard From(Tile tile, Coordinates coordinates) => new(tile, coordinates);
    public static TileOnBoard From(TileOnPlayer tileOnPlayer, Coordinates coordinates) => new(tileOnPlayer, coordinates);
    public static TileOnBoard From(TileOnBoard tile) => new(tile, tile.Coordinates);
    public static TileOnBoard From(TileShape shape, TileColor color, Coordinates coordinates) => new(new Tile(color, shape), coordinates);
    public static TileOnBoard From(TileShape shape, TileColor color, Abscissa x, Ordinate y) => new(new Tile(color, shape), Coordinates.From(x, y));
    private TileOnBoard(TileOnPlayer tileOnPlayer, Coordinates coordinates) : this(tileOnPlayer.ToTile(), coordinates) { }

    public Tile ToTile() => new(Color, Shape);

    public override string ToString() => $"{Tile} on {Coordinates}";
}