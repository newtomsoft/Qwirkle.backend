using Qwirkle.Core.ComplianceContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface IBagPersistence
    {
        List<Tile> GetAllTilesOfBag(int gameId);
        Tile GetRandomTileOfBag(int gameId);
        void SaveTile(int gameId, Tile tile);
        int CountAllTilesOfBag(int gameId);
        void DeleteAllTilesOfBag(int gameId);
    }
}