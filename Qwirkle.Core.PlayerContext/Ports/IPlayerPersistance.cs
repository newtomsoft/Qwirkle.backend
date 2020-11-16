using Qwirkle.Core.PlayerContext.Entities.Player;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.PlayerContext.Ports
{
    public interface IPlayerPersistence
    {
        Player GetPlayer(int playerId);
    }
}