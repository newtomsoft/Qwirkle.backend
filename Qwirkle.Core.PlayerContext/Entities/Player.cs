using System.Collections.Generic;

namespace Qwirkle.Core.PlayerContext.Entities.Player
{
    public class Player
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Pseudo { get; set; }
        public byte Points { get; set; }
        public byte GamePosition { get; set; }
        public bool GameTurn { get; set; }
        public Rack Rack { get; set; }
    }
}
