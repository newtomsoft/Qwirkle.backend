using Qwirkle.Core.BagContext.Entities;
using Qwirkle.Core.BagContext.Ports;
using Qwirkle.Core.CommonContext;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.BagContext.Services
{
    public class BagService : IRequestBagService
    {    
        private const int NUMBER_OF_TILES_IN_A_FULL_BAG = 108;
        private IBagPersistance Persistance { get; }

        public BagService(IBagPersistance persistance) => Persistance = persistance;

        public List<Tile> GetAllTilesOfBag(int gameId)
        {
            if (Persistance.CountAllTilesOfBag(gameId) != NUMBER_OF_TILES_IN_A_FULL_BAG)
            {
                DeleteAllTilesOfBag(gameId);
                SaveAllTilesOfBag(gameId);
            }
            return Persistance.GetAllTilesOfBag(gameId);
        }

        public Tile GetRandomTileOfBag(int gameId) => Persistance.GetRandomTileOfBag(gameId);


        private void SaveAllTilesOfBag(int gameId)
        {
            const int NUMBER_OF_SAME_TILE = 3;
            foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (TileForm form in (TileForm[])Enum.GetValues(typeof(TileForm)))
                    for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
                        Persistance.SaveTile(new Tile(0, gameId, color, form));
        }

        private void DeleteAllTilesOfBag(int gameId) => Persistance.DeleteAllTilesOfBag(gameId);
    }
}
