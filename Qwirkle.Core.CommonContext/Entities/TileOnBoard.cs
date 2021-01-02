using Qwirkle.Core.CommonContext.ValueObjects;

namespace Qwirkle.Core.CommonContext.Entities
{
    public class TileOnBoard : Tile
    {
        public CoordinatesInGame Coordinates { get; }

        public TileOnBoard(Tile tile, CoordinatesInGame coordinates) : base(tile)
        {
            Coordinates = coordinates;
        }

        public TileOnBoard(int id, TileColor color, TileForm form, CoordinatesInGame coordinates) : base(id, color, form)
        {
            Coordinates = coordinates;
        }

        public TileOnBoard(TileColor color, TileForm form, CoordinatesInGame coordinates) : this(0, color, form, coordinates)
        { }
    }
}
