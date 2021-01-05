using Qwirkle.Core.Enums;
using System;

namespace Qwirkle.Core.Entities
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
