using System.Collections.Generic;

namespace Qwirkle.Core.BoardContext.Entities
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
    }
}
