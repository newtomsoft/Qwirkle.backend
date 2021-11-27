namespace Qwirkle.Core.ValueObjects;

public record TileOnBag : Tile
{
    public TileOnBag(int id, TileColor color, TileShape shape) : base(id, color, shape)
    {

    }
}
