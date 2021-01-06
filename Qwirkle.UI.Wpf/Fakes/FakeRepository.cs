using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;
using System;
using System.Collections.Generic;

namespace Qwirkle.UI.Wpf.Fakes
{
    public class FakeRepository : IRepository
    {
        public Game CreateGame(DateTime date)
        {
            throw new NotImplementedException();
        }

        public Player CreatePlayer(int userId, int gameId)
        {
            throw new NotImplementedException();
        }

        public void CreateTiles(int gameId)
        {
            throw new NotImplementedException();
        }

        public Game GetGame(int gameId)
        {
            throw new NotImplementedException();
        }

        public Player GetPlayer(int playerId)
        {
            throw new NotImplementedException();
        }

        public Tile GetTileById(int tileId)
        {
            throw new NotImplementedException();
        }

        public TileOnPlayer GetTileOnPlayerById(int tileId)
        {
            throw new NotImplementedException();
        }

        public bool IsPlayerTurn(int playerId)
        {
            throw new NotImplementedException();
        }

        public void SetPlayerTurn(int playerId, bool turn)
        {
            throw new NotImplementedException();
        }

        public void TilesFromBagToPlayer(Player player, List<byte> rackPositions)
        {
            throw new NotImplementedException();
        }

        public void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles)
        {
            throw new NotImplementedException();
        }

        public void TilesFromPlayerToGame(int gameId, int playerId, List<TileOnBoard> tiles)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayer(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
