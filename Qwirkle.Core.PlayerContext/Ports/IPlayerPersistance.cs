using Qwirkle.Core.PlayerContext.Entities.Player;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IPlayerPersistence
    {
        Player GetPlayer(int playerId);
    }
}