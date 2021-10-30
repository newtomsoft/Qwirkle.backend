namespace Qwirkle.Core.Entities;

public class TileOnPlayer : Tile
{
    public RackPosition RackPosition { get; }

    public TileOnPlayer(RackPosition rackPosition, int id, TileColor color, TileShape shape) : base(id, color, shape)
    {
        RackPosition = rackPosition;
    }
}
