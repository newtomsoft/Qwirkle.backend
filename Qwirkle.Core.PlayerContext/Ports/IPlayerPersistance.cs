using Qwirkle.Core.CommonContext.Entities;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IPlayerPersistence
    {
        Player GetPlayer(int playerId);
    }
}