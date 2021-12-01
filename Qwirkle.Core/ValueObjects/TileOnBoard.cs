namespace Qwirkle.Core.ValueObjects;

public record TileOnBoard : Tile
{
    public Coordinates Coordinates { get; }

    public TileOnBoard(Tile tile, Coordinates coordinates) : base(tile) => Coordinates = coordinates;
    public TileOnBoard(TileColor color, TileShape shape, Coordinates coordinates) : base(color, shape) => Coordinates = coordinates;
    public static TileOnBoard From(TileOnPlayer tile, Coordinates coordinates) => new(tile, coordinates);

    private TileOnBoard(TileOnPlayer tile, Coordinates coordinates) : base(tile) => Coordinates = coordinates;

    public Tile ToTile() => new(Color, Shape);
}
