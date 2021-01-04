using Qwirkle.Core.ComplianceContext.Enums;
using System;

namespace Qwirkle.Core.ComplianceContext.Entities
{
    using RackPosition = Byte;
    public class TileOnPlayer : Tile
    {
        public RackPosition RackPosition { get; }

        public TileOnPlayer(RackPosition rackPosition, int id, TileColor color, TileForm form) : base(id, color, form)
        {
            RackPosition = rackPosition;
        }
    }
}
