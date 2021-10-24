using Qwirkle.Core.Entities;
using System;
using System.Collections.Generic;

namespace Qwirkle.Core.Ports;
public interface IRepository
{
    Game CreateGame(DateTime date);
    void SetPlayerTurn(int playerId);
    void UpdatePlayer(Player player);
    void TilesFromPlayerToGame(int gameId, int playerId, List<TileOnBoard> tiles);
    void TilesFromBagToPlayer(Player player, List<byte> positionsInRack);
    void TilesFromPlayerToBag(Player player, List<TileOnPlayer> tiles);
    Game GetGame(int gameId);
    Player GetPlayer(int playerId);
    string GetPlayerNameTurn(int gameId);
    int GetPlayerIdToPlay(int gameId);
    Tile GetTileById(int tileId);
    TileOnPlayer GetTileOnPlayerById(int tileId);
    List<int> GetListGameIDWithPlayer();
    List<string> GetListNamePlayer(int gameId);
    Player CreatePlayer(int userId, int gameId);
    void CreateTiles(int gameId);
    void SetGameOver(int gameId);
    List<int> GetLeadersPlayersId(int gameId);
    bool IsGameOver(int gameId);
    void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange);
    List<int> GetUsersId();
}
