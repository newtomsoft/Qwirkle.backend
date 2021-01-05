using System.Collections.Generic;

namespace Qwirkle.Core.Entities
{
    public class Bag
    {
        public int Id { get; set; }
        public List<TileOnBag> Tiles { get; set; }

        public Bag(int id, List<TileOnBag> tiles = null)
        {
            Id = id;
            Tiles = tiles ?? new List<TileOnBag>();
        }
    }
}