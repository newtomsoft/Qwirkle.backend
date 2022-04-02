namespace Qwirkle.Domain.ValueObjects;

[Serializable]
public record TileOnBoard(Tile Tile, Coordinate Coordinate) : Tile(Tile)
{
    public TileOnBoard(TileColor color, TileShape shape, Coordinate coordinate) : this(new Tile(color, shape), coordinate) { }
    public static TileOnBoard From(Tile tile, Coordinate coordinate) => new(tile, coordinate);
    public static TileOnBoard From(TileOnRack tileOnRack, Coordinate coordinate) => new(tileOnRack, coordinate);
    public static TileOnBoard From(TileOnBoard tile) => new(tile, tile.Coordinate);
    public static TileOnBoard From(TileShape shape, TileColor color, Coordinate coordinate) => new(new Tile(color, shape), coordinate);
    public static TileOnBoard From(TileShape shape, TileColor color, Abscissa x, Ordinate y) => new(new Tile(color, shape), Coordinate.From(x, y));
    private TileOnBoard(TileOnRack tileOnRack, Coordinate coordinate) : this(tileOnRack.ToTile(), coordinate) { }

    public TileOnBoard() : this(new Tile(), Coordinate.From(0, 0)) { }

    public Tile ToTile() => new(Color, Shape);

    public override string ToString() => $"{Tile} on {Coordinate}";
}