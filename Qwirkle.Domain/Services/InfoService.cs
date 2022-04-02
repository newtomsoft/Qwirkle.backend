namespace Qwirkle.Domain.Services;

public class InfoService
{
    private readonly IRepository _repository;
    private readonly INotification _notification;
    private readonly ILogger<InfoService> _logger;

    public InfoService(IRepository repository, INotification notification, ILogger<InfoService> logger)
    {
        _repository = repository;
        _notification = notification;
        _logger = logger;
    }

    public int GetUserId(string userName) => _repository.GetUserId(userName);
    public int GetPlayerId(int gameId, int userId) => _repository.GetPlayerId(gameId, userId);
    public Player GetPlayer(int playerId) => _repository.GetPlayer(playerId);
    public Player GetPlayer(int gameId, int userId) => _repository.GetPlayer(gameId, userId);
    public string GetPlayerNameTurn(int gameId) => _repository.GetPlayerNameTurn(gameId);
    public int GetPlayerIdTurn(int gameId) => _repository.GetPlayerIdToPlay(gameId);
    public List<int> GetGamesIdsContainingPlayers() => _repository.GetGamesIdsContainingPlayers();
    public List<int> GetAllUsersId() => _repository.GetAllUsersId();
    public List<int> GetUserGames(int userId) => _repository.GetUserGamesIds(userId);
    public Game GetGameForSuperUser(int gameId) => _repository.GetGame(gameId);
    public Game GetGame(int gameId) => _repository.GetGame(gameId);
    public async Task<Game> GetGameAsync(int gameId) => await _repository.GetGameAsync(gameId);
    public List<int> GetWinnersPlayersId(int gameId)
    {
        if (!_repository.IsGameOver(gameId)) return new List<int>();
        var winnersPlayersIds = _repository.GetLeadersPlayersId(gameId);
        _notification.SendGameOver(gameId, winnersPlayersIds);
        return winnersPlayersIds;
    }
    public Game GetGameWithTilesOnlyForAuthenticatedUser(int gameId, int userId)
    {
        var game = _repository.GetGame(gameId);
        var isUserInGame = game.Players.Any(p => p.UserId == userId);
        if (!isUserInGame) return null;
        var otherPlayers = game.Players.Where(p => p.UserId != userId);
        foreach (var player in otherPlayers) player.Rack = player.Rack.ToHiddenRack();
        return game;
    }
}