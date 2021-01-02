

using Qwirkle.Core.CommonContext.Entities;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IRequestGame
    {
        Game GetGame(int gameId);
    }
}
