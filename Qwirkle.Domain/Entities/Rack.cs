namespace Qwirkle.Domain.Entities;

public class Rack
{
    public List<TileOnPlayer> Tiles { get; }

    public Rack(List<TileOnPlayer> tiles)
    {
        Tiles = tiles;
    }

    public Rack WithoutDuplicatesTiles() => new(Tiles.Distinct().ToList());
}