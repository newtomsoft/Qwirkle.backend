using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Board
    {
        public int Id { get; }
        public List<Tile> Tiles { get; set; }
        public List<Player> Players { get; set; }

        public Board(int id, List<Tile> tiles, List<Player> players = null)
        {
            Id = id;
            Tiles = tiles;
            Players = players;
        }

        public Board(Board board)
        {
            Id = board.Id;
            Tiles = board.Tiles.ToList();
            Players = board.Players;
        }
    }
}
