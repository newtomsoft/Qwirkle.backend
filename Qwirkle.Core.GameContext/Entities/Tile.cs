using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;

namespace Qwirkle.Core.GameContext.Entities
{
    public class Tile
    {
        public int Id { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }
        public CoordinatesInGame Coordinates { get; }

        public Tile(int id, TileColor color, TileForm form, CoordinatesInGame coordinates)
        {
            Id = id;
            Color = color;
            Form = form;
            Coordinates = coordinates;
        }

        public Tile(TileColor color, TileForm form, CoordinatesInGame coordinates) : this(0, color, form, coordinates)
        { }

        public Tile(int id)
        {
            Id = id;
        }
    }
}
