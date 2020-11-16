using Qwirkle.Core.GameContext.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.GameContext.Ports
{
    public interface IGamePersistence
    {
        Game GetGame(int gameId);
    }
}