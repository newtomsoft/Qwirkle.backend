using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwirkle.Core.CommonContext.Entities
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
