using Qwirkle.Core.CommonContext;

namespace Qwirkle.Core.BagContext.Entities
{
    public class Tile
    {
        public int Id { get; }
        public int GameId { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }

        public Tile(int id, int gameId, TileColor color, TileForm form)
        {
            Id = id;
            GameId = gameId;
            Color = color;
            Form = form;
        }

        //public Tile(int gameId, TileColor color, TileForm form) : this(0, gameId, color, form)
        //{ }
    }
}
