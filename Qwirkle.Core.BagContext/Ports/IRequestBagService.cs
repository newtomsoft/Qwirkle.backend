using Qwirkle.Core.CommonContext.Entities;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Ports
{
    public interface IRequestBagService
    {
        List<Tile> GetAllTilesOfBag(int gameId);
        Tile GetRandomTileOfBag(int gameId);
    }
}