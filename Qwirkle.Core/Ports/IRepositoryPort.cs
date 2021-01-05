using Qwirkle.Core.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.Ports
{
    public interface IRepositoryPort
    {
        Game CreateGame(DateTime date);
        bool IsPlayerTurn(int playerId);
        void SetPlayerTurn(int playerId, bool turn);
        void UpdatePlayer(Player player);
        void TilesFromPlayerToGame(int gameId, int playerId, List<TileOnBoard> tiles);
        void TilesFromBagToPlayer(Player player, List<byte> rackPositions);
        void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles);
        Game GetGame(int gameId);
        Player GetPlayer(int playerId);
        Tile GetTileById(int tileId);
        TileOnPlayer GetTileOnPlayerById(int tileId);
        Player CreatePlayer(int userId, int gameId);
        void CreateTiles(int gameId);
    }
}