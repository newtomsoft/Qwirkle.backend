using System.Collections.Generic;

namespace Qwirkle.Core.GameContext.Entities
{
    public class Bag
    {
        public int Id { get; set; }
        public List<Tile> Tiles { get; set; }
    }
}