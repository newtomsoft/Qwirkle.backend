using Qwirkle.Core.ComplianceContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IRequestComplianceService
    {
        int PlayTiles(Board board, Player player, List<Tile> tiles);
    }
}