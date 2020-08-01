using Qwirkle.Core.ComplianceContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IRequestComplianceService
    {
        List<Player> CreateGame(List<int> usersIds);
        int PlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay);
    }
}