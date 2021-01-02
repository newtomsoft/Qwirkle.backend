using Qwirkle.Core.CommonContext.Entities;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IGamePersistence
    {
        Game GetGame(int gameId);
    }
}