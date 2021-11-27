namespace Qwirkle.Core.ValueObjects;

public record TileOnPlayer : Tile
{
    public RackPosition RackPosition { get; }

    public TileOnPlayer(RackPosition rackPosition, int id, TileColor color, TileShape shape) : base(id, color, shape) => RackPosition = rackPosition;

    public TileOnBoard ToTileOnBoard(Coordinates coordinates) => TileOnBoard.From(this, coordinates);
}
