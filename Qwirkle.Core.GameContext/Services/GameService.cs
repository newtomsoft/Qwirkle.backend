using Qwirkle.Core.CommonContext.Entities;
using Qwirkle.Core.GameContext.Ports;

namespace Qwirkle.Core.GameContext.Services
{
    public class GameService : IRequestGame
    {
        private IGamePersistence PersistenceAdapter { get; }


        public GameService(IGamePersistence persistenceAdapter) => PersistenceAdapter = persistenceAdapter;

        public Game GetGame(int gameId)
        {
            return PersistenceAdapter.GetGame(gameId);
        }
    }
}
