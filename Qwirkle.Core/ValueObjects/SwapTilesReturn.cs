using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;

namespace Qwirkle.Core.ValueObjects
{
    public struct SwapTilesReturn
    {
        public PlayReturnCode Code { get; set; }
        public Rack NewRack { get; set; }
    }
}
