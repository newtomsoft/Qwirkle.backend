using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int GamePosition { get; set; }
        public bool GameTurn { get; set; }
        public int Points { get; set; }
        public List<Tile> Tiles { get; set; }
    }
}
