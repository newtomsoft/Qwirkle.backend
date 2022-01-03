namespace Qwirkle.Domain.Ports;

public interface IRepository
{
    Game CreateGame(DateTime date);
    void SetPlayerTurn(int playerId);
    void UpdatePlayer(Player player);
    void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<TileOnBoard> tilesOnBoard);
    void TilesFromBagToPlayer(Player player, List<byte> positionsInRack);
    void TilesFromPlayerToBag(Player player, IEnumerable<Tile> tiles);
    Game GetGame(int gameId);
    int GetPlayerId(int gameId, int userId);
    Player GetPlayer(int playerId);
    Player GetPlayer(int gameId, int userId);
    string GetPlayerNameTurn(int gameId);
    int GetPlayerIdToPlay(int gameId);
    int GetUserId(int playerId);
    int GetUserId(string userName);
    Tile GetTile(TileColor color, TileShape shape);
    List<int> GetGamesIdsContainingPlayers();
    Player CreatePlayer(int userId, int gameId);
    void CreateTiles(int gameId);
    void SetGameOver(int gameId);
    List<int> GetLeadersPlayersId(int gameId);
    bool IsGameOver(int gameId);
    void ArrangeRack(Player player, IEnumerable<Tile> tiles);
    List<int> GetAllUsersId();
    List<int> GetUserGamesIds(int userId);
}
