﻿namespace Qwirkle.Domain.Ports;
public interface IRepository
{
    Game CreateGame(DateTime date);
    void SetPlayerTurn(int playerId);
    void UpdatePlayer(Player player);
    void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay);
    void TilesFromBagToPlayer(Player player, List<byte> positionsInRack);
    void TilesFromPlayerToBag(Player player, IEnumerable<int> tilesIds);
    Game GetGame(int gameId);
    Player GetPlayer(int playerId);
    Player GetPlayer(int gameId, int userId);
    string GetPlayerNameTurn(int gameId);
    int GetPlayerIdToPlay(int gameId);
    int GetUserId(int playerId);
    Tile GetTile(int id);
    List<int> GetGamesIdsContainingPlayers();
    Player CreatePlayer(int userId, int gameId);
    void CreateTiles(int gameId);
    void SetGameOver(int gameId);
    List<int> GetLeadersPlayersId(int gameId);
    bool IsGameOver(int gameId);
    void ArrangeRack(Player player, List<int> tilesIds);
    List<int> GetAllUsersId();
    List<int> GetUserGamesIds(int userId);
}