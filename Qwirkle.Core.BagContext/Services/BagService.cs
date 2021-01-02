using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Services
{
    public class BagService : IRequestBagService
    {
        private const int NUMBER_OF_TILES_IN_A_FULL_BAG = 108;
        private IBagPersistence Persistence { get; }

        public BagService(IBagPersistence persistence) => Persistence = persistence;

        public List<Tile> GetAllTilesOfBag(int gameId)
        {
            if (Persistence.CountAllTilesOfBag(gameId) != NUMBER_OF_TILES_IN_A_FULL_BAG)
            {
                DeleteAllTilesOfBag(gameId);
                SaveAllTilesOfBag(gameId);
            }
            return Persistence.GetAllTilesOfBag(gameId);
        }

        public Tile GetRandomTileOfBag(int gameId) => Persistence.GetRandomTileOfBag(gameId);


        private void SaveAllTilesOfBag(int gameId)
        {
            const int NUMBER_OF_SAME_TILE = 3;
            foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                    for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                        Persistence.SaveTile(gameId, new Tile(0, color, form));
        }

        private void DeleteAllTilesOfBag(int gameId) => Persistence.DeleteAllTilesOfBag(gameId);
    }
}
