using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Player
    {
        private bool _gameTurn;

        public int Id { get; set; }
        public int GameId { get; set; }
        public int GamePosition { get; set; }
        public int Points { get; set; }
        public List<Tile> Tiles { get; set; }

        public Player(int id, int gameId, int gamePosition, int points, List<Tile> tiles, bool turn)
        {
            Id = id;
            GameId = gameId;
            GamePosition = gamePosition;
            Points = points;
            Tiles = tiles;
            _gameTurn = turn;
        }

        public bool IsTurn => _gameTurn;
        public void SetTurn(bool turn) => _gameTurn = turn;
        public bool HasTiles(List<Tile> tiles)
        {
            var playerTilesId = Tiles.Select(t => t.Id);
            var tilesId = tiles.Select(t => t.Id);
            return tilesId.All(id => playerTilesId.Contains(id));
        }
    }
}
