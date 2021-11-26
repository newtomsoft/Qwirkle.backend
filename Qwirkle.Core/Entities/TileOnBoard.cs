namespace Qwirkle.Core.Entities;

public class TileOnBoard : Tile
{
    public Coordinates Coordinates { get; }

    public TileOnBoard(Tile tile, Coordinates coordinates) : base(tile) => Coordinates = coordinates;
    public TileOnBoard(int id, TileColor color, TileShape shape, Coordinates coordinates) : base(id, color, shape) => Coordinates = coordinates;
    public TileOnBoard(TileColor color, TileShape shape, Coordinates coordinates) : this(0, color, shape, coordinates) { }
    public static TileOnBoard From(TileOnPlayer tile, Coordinates coordinates) => new (tile, coordinates);
    
    private TileOnBoard(TileOnPlayer tile, Coordinates coordinates) : base(tile) => Coordinates = coordinates;
}
