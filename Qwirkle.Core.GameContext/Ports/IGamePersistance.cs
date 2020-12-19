using Qwirkle.Core.GameContext.Entities;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IGamePersistence
    {
        Game GetGame(int gameId);
    }
}