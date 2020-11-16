using Qwirkle.Core.PlayerContext.Entities.Player;
using Qwirkle.Core.PlayerContext.Ports;

namespace Qwirkle.Core.PlayerContext.Services
{
    public class PlayerService : IRequestPlayer
    {
        private IPlayerPersistence PersistenceAdapter { get; }


        public PlayerService(IPlayerPersistence persistenceAdapter) => PersistenceAdapter = persistenceAdapter;

        public Player GetPlayer(int playerId)
        {
            return PersistenceAdapter.GetPlayer(playerId);
        }
    }
}
