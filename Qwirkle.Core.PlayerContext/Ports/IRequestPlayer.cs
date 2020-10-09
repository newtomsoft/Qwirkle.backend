using Qwirkle.Core.PlayerContext.Entities.Player;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IRequestPlayer
    {
        Player GetPlayer(int playerId);
    }
}
