using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;

namespace Qwirkle.Core.ComplianceContext.Services
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
