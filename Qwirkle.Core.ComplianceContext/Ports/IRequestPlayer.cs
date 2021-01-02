using Qwirkle.Core.ComplianceContext.Entities;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IRequestPlayer
    {
        Player GetPlayer(int playerId);
    }
}
