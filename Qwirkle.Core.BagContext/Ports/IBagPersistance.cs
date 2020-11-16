using Qwirkle.Core.BagContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Ports
{
    public interface IBagPersistence
    {
        List<Tile> GetAllTilesOfBag(int gameId);
        Tile GetRandomTileOfBag(int gameId);
        void SaveTile(Tile tile);
        int CountAllTilesOfBag(int gameId);
        void DeleteAllTilesOfBag(int gameId);
    }
}