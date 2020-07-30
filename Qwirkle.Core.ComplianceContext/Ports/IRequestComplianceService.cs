using Qwirkle.Core.ComplianceContext.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IRequestComplianceService
    {
        int PlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay);
    }
}