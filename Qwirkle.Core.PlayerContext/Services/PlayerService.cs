using Qwirkle.Core.PlayerContext.Entities.Player;
using Qwirkle.Core.PlayerContext.Ports;

namespace Qwirkle.Core.PlayerContext.Services
{
    public class PlayerService : IRequestPlayerService
    {
        private IPlayerPersistance Persistance { get; }


        public PlayerService(IPlayerPersistance persistance) => Persistance = persistance;

        public Player GetPlayer(int playerId)
        {
            return Persistance.GetPlayer(playerId);
        }
    }
}
