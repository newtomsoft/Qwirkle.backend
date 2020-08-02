using Qwirkle.Core.PlayerContext.Entities.Player;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IRequestPlayerService
    {
        Player GetPlayer(int playerId);
    }
}
