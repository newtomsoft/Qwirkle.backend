using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.Entities
{
    public class Board
    {
        public List<TileOnBoard> Tiles { get; set; }

        public Board(List<TileOnBoard> tiles)
        {
            Tiles = tiles;
        }

        public bool IsIsolatedTile(TileOnBoard tile)
        {
            var tileRight = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Right());
            var tileLeft = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Left());
            var tileTop = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Top());
            var tileBottom = Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Bottom());
            return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
        }
    }
}
