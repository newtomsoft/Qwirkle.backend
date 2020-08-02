using Qwirkle.Core.GameContext.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IGamePersistance
    {
        Game GetGame(int gameId);
    }
}