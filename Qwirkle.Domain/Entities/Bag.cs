namespace Qwirkle.Domain.Entities;

public class Bag
{
    public int Id { get; }
    public List<TileOnBag> Tiles { get; }

    public Bag(int id, List<TileOnBag> tiles = null)
    {
        Id = id;
        Tiles = tiles ?? new List<TileOnBag>();
    }

    public static Bag Empty() => new(0, new List<TileOnBag>());

}