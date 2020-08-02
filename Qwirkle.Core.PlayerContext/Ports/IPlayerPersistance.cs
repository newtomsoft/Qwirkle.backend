using Qwirkle.Core.PlayerContext.Entities.Player;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IPlayerPersistance
    {
        Player GetPlayer(int playerId);
    }
}