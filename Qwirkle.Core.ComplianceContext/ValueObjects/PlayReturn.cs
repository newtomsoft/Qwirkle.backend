using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.Entities;
using Qwirkle.Core.CommonContext.Enums;

namespace Qwirkle.Core.ComplianceContext.ValueObjects
{
    public struct PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public TileOnBoard Tile { get; set; }
        public int Points { get; set; }
    }
}
