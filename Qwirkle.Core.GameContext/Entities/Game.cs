using System.Collections.Generic;

namespace Qwirkle.Core.GameContext.Entities
{
    public class Game
    {
        public int Id { get; }
        public List<Tile> Tiles { get; }
        public Bag Bag { get; }

        public Game()
        {
        }

        public Game(int id, List<Tile> tiles)
        {
            Id = id;
            Tiles = tiles;
        }

        public Game(int id, List<Tile> tiles, Bag bag)
        {
            Id = id;
            Tiles = tiles;
            Bag = bag;
        }
    }
}
