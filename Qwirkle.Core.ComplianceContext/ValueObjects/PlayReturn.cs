using Qwirkle.Core.ComplianceContext;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Enums;

namespace Qwirkle.Core.ComplianceContext.ValueObjects
{
    public struct PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public TileOnBoard Tile { get; set; }
        public int Points { get; set; }
    }
}
