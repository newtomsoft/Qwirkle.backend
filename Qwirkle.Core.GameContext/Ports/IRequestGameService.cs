using Qwirkle.Core.GameContext.Entities;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IRequestGameService
    {
        Game GetGame(int gameId);
    }
}
