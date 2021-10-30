namespace Qwirkle.Core.Entities;

public class TileOnBoard : Tile
{
    public CoordinatesInGame Coordinates { get; }

    public TileOnBoard(Tile tile, CoordinatesInGame coordinates) : base(tile) => Coordinates = coordinates;

    public TileOnBoard(int id, TileColor color, TileShape shape, CoordinatesInGame coordinates) : base(id, color, shape) => Coordinates = coordinates;

    public TileOnBoard(TileColor color, TileShape shape, CoordinatesInGame coordinates) : this(0, color, shape, coordinates)
    { }
}
