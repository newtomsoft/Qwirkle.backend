using Qwirkle.Core.CommonContext.Entities;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IRequestPlayer
    {
        Player GetPlayer(int playerId);
    }
}
