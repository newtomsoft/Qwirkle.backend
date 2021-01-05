using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;

namespace Qwirkle.Core.ValueObjects
{
    public struct PlayReturn
    {
        public PlayReturnCode Code { get; set; }
        public TileOnBoard Tile { get; set; }
        public int Points { get; set; }
    }
}
