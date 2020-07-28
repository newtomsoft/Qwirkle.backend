using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Board
    {
        public int Id { get; }
        public List<Tile> Tiles { get; }


        public Board(int id, List<Tile> tiles)
        {
            Id = id;
            Tiles = tiles;
        }

        public Board(Board board)
        {
            Id = board.Id;
            Tiles = board.Tiles.ToList();
        }
    }
}
