using Qwirkle.Core.ComplianceContext;
using Qwirkle.Core.ComplianceContext.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
