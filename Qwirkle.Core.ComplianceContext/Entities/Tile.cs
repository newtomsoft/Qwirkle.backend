using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    public class Tile
    {
        public int Id { get; }
        public TileColor Color { get; }
        public TileForm Form { get; }
        public CoordinatesInGame Coordinates { get; set; }

        public Tile(int id, TileColor color, TileForm form)
        {
            Id = id;
            Color = color;
            Form = form;
        }
        public Tile(int id, TileColor color, TileForm form, CoordinatesInGame coordinates) : this(id, color, form)
        {
            Coordinates = coordinates;
        }

        public Tile(TileColor color, TileForm form, CoordinatesInGame coordinates) : this(0, color, form, coordinates)
        { }

        public Tile(TileColor color, TileForm form) : this(0, color, form)
        { }

        public Tile(int id)
        {
            Id = id;
        }

        public bool HaveFormOrColorOnlyEqual(Tile tile)
        {
            if (Color == tile.Color && Form != tile.Form || Color != tile.Color && Form == tile.Form)
                return true;
            return false;
        }
    }
}
