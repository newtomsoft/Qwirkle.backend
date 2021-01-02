using Qwirkle.Core.ComplianceContext.Entities;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IPlayerPersistence
    {
        Player GetPlayer(int playerId);
    }
}