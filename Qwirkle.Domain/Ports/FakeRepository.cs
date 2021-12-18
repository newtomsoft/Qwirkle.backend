namespace Qwirkle.Domain.Ports;

public class FakeRepository : IRepository
{
    public Game CreateGame(DateTime date) => throw new NotImplementedException();
    public Player CreatePlayer(int userId, int gameId) => throw new NotImplementedException();
    public void CreateTiles(int gameId) => throw new NotImplementedException();
    public List<int> GetAllUsersId() => throw new NotImplementedException();
    public Game GetGame(int gameId) => throw new NotImplementedException();
    public int GetPlayerId(int gameId, int userId) => throw new NotImplementedException();
    public Tile GetTile(TileColor color, TileShape shape) => throw new NotImplementedException();
    public List<int> GetGamesIdsContainingPlayers() => throw new NotImplementedException();
    public List<int> GetLeadersPlayersId(int gameId) => throw new NotImplementedException();
    public Player GetPlayer(int playerId) => throw new NotImplementedException();
    public Player GetPlayer(int gameId, int userId) => throw new NotImplementedException();
    public int GetPlayerIdToPlay(int gameId) => throw new NotImplementedException();
    public string GetPlayerNameTurn(int gameId) => throw new NotImplementedException();
    public List<int> GetUserGamesIds(int userId) => throw new NotImplementedException();
    public int GetUserId(int playerId) => throw new NotImplementedException();
    public int GetUserId(string userName) => throw new NotImplementedException();
    public bool IsGameOver(int gameId) => throw new NotImplementedException();
    public void SetGameOver(int gameId) => throw new NotImplementedException();
    public void SetPlayerTurn(int playerId) => throw new NotImplementedException();
    public void TilesFromBagToPlayer(Player player, List<byte> positionsInRack) => throw new NotImplementedException();
    public void UpdatePlayer(Player player) => throw new NotImplementedException();
    public void TilesFromPlayerToBag(Player player, IEnumerable<(TileColor color, TileShape shape)> tilesTupleToPlay) => throw new NotImplementedException();
    public void ArrangeRack(Player player, IEnumerable<(TileColor color, TileShape shape)> tilesTuple) => throw new NotImplementedException();
    public void TilesFromPlayerToBoard(int gameId, int playerId, IEnumerable<TileOnBoard> tilesOnBoard) => throw new NotImplementedException();
}
