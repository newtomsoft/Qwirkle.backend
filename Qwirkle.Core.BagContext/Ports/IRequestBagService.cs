using Qwirkle.Core.BagContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Ports
{
    public interface IRequestBagService
    {
        List<Tile> GetAllTilesOfBag(int gameId);
        Tile GetRandomTileOfBag(int gameId);
    }
}