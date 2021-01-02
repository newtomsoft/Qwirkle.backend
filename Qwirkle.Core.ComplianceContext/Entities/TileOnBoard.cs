using Qwirkle.Core.ComplianceContext.Enums;
using Qwirkle.Core.ComplianceContext.ValueObjects;

namespace Qwirkle.Core.ComplianceContext.Entities
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
