using Qwirkle.Core.CommonContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Ports
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