namespace Qwirkle.Domain.Entities;

public class Bag
{
    public int Id { get; }
    public List<TileOnBag> Tiles { get; }

    public int TilesNumber => Tiles.Count;

    public Bag(int id, List<TileOnBag> tiles = null)
    {
        Id = id;
        Tiles = tiles ?? new List<TileOnBag>();
    }

    public static Bag Empty => new(0, new List<TileOnBag>());

    public static Bag WithFakeTiles(int tilesNumber)
    {
        var tiles = new List<TileOnBag>();
        for (var i = 0; i < tilesNumber; i++)
            tiles.Add(new TileOnBag(TileColor.Green, TileShape.Circle));
        return new(0, tiles);
    }
}