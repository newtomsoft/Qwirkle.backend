using Qwirkle.Core.CommonContext;

namespace Qwirkle.Infra.Persistance.Models
{
    public class TileOnBoard
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public TileForm Form { get; set; }
        public TileColor Color { get; set; }
        public byte PositionX { get; set; }
        public byte PositionY { get; set; }

        public virtual Game Game { get; set; }
    }
}
