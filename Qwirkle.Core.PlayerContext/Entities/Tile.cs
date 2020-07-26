using Qwirkle.Core.CommonContext;

namespace Qwirkle.Core.PlayerContext.Entities
{
    public class Tile
    {
        public int Id { get; }
        public byte OrderNumber { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }


        public Tile(int id, TileColor color, TileForm form, byte orderNumber)
        {
            Id = id;
            OrderNumber = orderNumber;
            Color = color;
            Form = form;
        }
    }
}
