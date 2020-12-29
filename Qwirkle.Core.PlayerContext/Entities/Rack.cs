using Qwirkle.Core.CommonContext;

namespace Qwirkle.Core.PlayerContext.Entities
{
    public class Rack
    {
        public Tile[] Tiles { get; }

        public Rack(Tile[] tiles)
        {
            Tiles = tiles;
        }
    }
}
