using Qwirkle.Core.ComplianceContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface ICompliancePersistance
    {
        bool IsPlayerTurn(int gameId, int playerId);
        void SetPlayerTurn(int gameId, int playerId);

    }
}