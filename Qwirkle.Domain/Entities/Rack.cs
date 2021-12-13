namespace Qwirkle.Domain.Entities;

public class Rack
{
    public List<TileOnPlayer> Tiles { get; }

    public static Rack From(List<TileOnPlayer> tiles) => new(tiles);

       public Rack(List<TileOnPlayer> tiles)
    {
        Tiles = tiles!=null ? tiles.ConvertAll(x=>new TileOnPlayer(x.RackPosition,x.Color,x.Shape)) : null;
    }


    public Rack WithoutDuplicatesTiles() => new(Tiles.Distinct().ToList());
}