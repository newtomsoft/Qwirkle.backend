using Qwirkle.Core.GameContext.Entities;
using Qwirkle.Core.GameContext.Ports;

namespace Qwirkle.Core.GameContext.Services
{
    public class GameService : IRequestGameService
    {
        private IGamePersistance Persistance { get; }


        public GameService(IGamePersistance persistance) => Persistance = persistance;

        public Game GetGame(int gameId)
        {
            return Persistance.GetGame(gameId);
        }
    }
}
