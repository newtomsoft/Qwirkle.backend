namespace Qwirkle.Domain.Ports;

public class NoRepository : IRepository
{
    public Game CreateGame(DateTime date) => throw new NotSupportedException();
    public Player CreatePlayer(int userId, int gameId) => throw new NotSupportedException();
    public void PutTilesOnBag(int gameId) => throw new NotSupportedException();
    public List<int> GetAllUsersId() => throw new NotSupportedException();
    public Game GetGame(int gameId) => throw new NotSupportedException();
    public Task<Game> GetGameAsync(int gameId) => throw new NotSupportedException();
    public int GetPlayerId(int gameId, int userId) => throw new NotSupportedException();
    public List<int> GetGamesIdsContainingPlayers() => throw new NotSupportedException();
    public List<int> GetLeadersPlayersId(int gameId) => throw new NotSupportedException();
    public Player GetPlayer(int playerId) => throw new NotSupportedException();
    public Player GetPlayer(int gameId, int userId) => throw new NotSupportedException();
    public int GetPlayerIdToPlay(int gameId) => throw new NotSupportedException();
    public string GetPlayerNameTurn(int gameId) => throw new NotSupportedException();
    public List<int> GetUserGamesIds(int userId) => throw new NotSupportedException();
    public int GetUserId(string userName) => throw new NotSupportedException();
    public bool IsGameOver(int gameId) => throw new NotSupportedException();
    public void SetGameOver(int gameId) => throw new NotSupportedException();
    public void SetPlayerTurn(int playerId) => throw new NotSupportedException();
    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack) => throw new NotSupportedException();
    public void UpdatePlayer(Player player) => throw new NotSupportedException();
    public void ArrangeRack(Player player, IEnumerable<Tile> tiles) => throw new NotSupportedException();
    public void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<TileOnBoard> tilesOnBoard) => throw new NotSupportedException();
    public void TilesFromPlayerToBag(Player player, IEnumerable<Tile> tiles) => throw new NotSupportedException();
    public bool AddBookmarkedOpponent(int userId, string opponentName) => throw new NotSupportedException();
    public bool RemoveBookmarkedOpponent(int userId, string opponentName) => throw new NotSupportedException();
    public HashSet<string> GetBookmarkedOpponentsNames(int userId) => throw new NotSupportedException();
}