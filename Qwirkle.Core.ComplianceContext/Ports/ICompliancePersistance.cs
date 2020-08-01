using Qwirkle.Core.ComplianceContext.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface ICompliancePersistance
    {
        bool IsPlayerTurn(int playerId);
        void SetPlayerTurn(int playerId, bool turn);
        void TilesFromPlayerToGame(int gameId, List<Tile> tiles);
        void UpdatePlayer(Player player);
        void TilesFromBagToPlayer(Player player, int tilesNumber);
        void TilesFromPlayerToBag(Player player, List<Tile> tiles);
        Game GetGame(int gameId);
        Player GetPlayerById(int playerId);
        Tile GetTileById(int tileId);
        Game CreateGame(DateTime date);
        Player CreatePlayer(int userId, int gameId);
        void CreateTiles(int gameId);
    }
}