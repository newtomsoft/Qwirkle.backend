using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.ValueObjects;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.ComplianceContext.Ports
{
    public interface ICompliancePersistence
    {
        Game CreateGame(DateTime date);
        bool IsPlayerTurn(int playerId);
        void SetPlayerTurn(int playerId, bool turn);
        void UpdatePlayer(Player player);
        void TilesFromPlayerToGame(int gameId, int playerId, List<Tile> tiles);
        void TilesFromBagToPlayer(Player player, int tilesNumber);
        void TilesFromPlayerToBag(Player player, List<Tile> tiles);
        Game GetGame(int gameId);
        Player GetPlayer(int playerId);
        Tile GetTileById(int tileId);
        Player CreatePlayer(int userId, int gameId);
        void CreateTiles(int gameId);
    }
}