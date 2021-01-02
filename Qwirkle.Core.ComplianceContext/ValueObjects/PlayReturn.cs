using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.Entities;

namespace Qwirkle.Core.ComplianceContext.ValueObjects
{
    public struct PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public TileOnBoard Tile { get; set; }
        public int Points { get; set; }
    }
}
