using System.Collections.Generic;

namespace Qwirkle.Core.Entities
{
    public class Rack
    {
        public List<TileOnPlayer> Tiles { get; }

        public Rack(List<TileOnPlayer> tiles)
        {
            Tiles = tiles;
        }
    }
}
