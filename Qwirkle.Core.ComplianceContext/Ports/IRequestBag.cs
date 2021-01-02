using Qwirkle.Core.ComplianceContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IRequestBag
    {
        List<Tile> GetAllTilesOfBag(int gameId);
        Tile GetRandomTileOfBag(int gameId);
    }
}