using Qwirkle.Core.PlayerContext.Entities.Player;
using Qwirkle.Core.PlayerContext.Ports;

namespace Qwirkle.Core.PlayerContext.Services
{
    public class PlayerService : IRequestPlayer
    {
        private IPlayerPersistance PersistanceAdapter { get; }


        public PlayerService(IPlayerPersistance persistanceAdapter) => PersistanceAdapter = persistanceAdapter;

        public Player GetPlayer(int playerId)
        {
            return PersistanceAdapter.GetPlayer(playerId);
        }
    }
}
