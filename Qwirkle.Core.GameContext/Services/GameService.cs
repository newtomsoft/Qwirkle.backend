using Qwirkle.Core.GameContext.Entities;
using Qwirkle.Core.GameContext.Ports;

namespace Qwirkle.Core.GameContext.Services
{
    public class GameService : IRequestGame
    {
        private IGamePersistance PersistanceAdapter { get; }


        public GameService(IGamePersistance persistanceAdapter) => PersistanceAdapter = persistanceAdapter;

        public Game GetGame(int gameId)
        {
            return PersistanceAdapter.GetGame(gameId);
        }
    }
}
