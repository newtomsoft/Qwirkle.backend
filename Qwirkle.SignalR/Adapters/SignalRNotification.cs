namespace Qwirkle.SignalR.Adapters;

public class SignalRNotification : INotification
{
    private readonly IHubContext<SignalRHub> _hubContextQwirkle;

    public SignalRNotification(IHubContext<SignalRHub> hubContextQwirkle)
    {
        _hubContextQwirkle = hubContextQwirkle;
    }

    public void SendPlayerIdTurn(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceivePlayerIdTurn, playerId);
    public void SendTurnSkipped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceiveTurnSkipped, playerId);
    public void SendTilesPlayedOld(int gameId, int playerId, Move move) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceiveTilesPlayed, playerId, move.Points, move.Tiles); //TODO remove
    public void SendTilesPlayed(int gameId, int playerId, Move move) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceiveTilesPlayedNew, playerId, move);
    public void SendTilesSwapped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceiveTilesSwapped, playerId);
    public void SendGameOver(int gameId, List<int> winnersPlayersIds) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync(INotification.ReceiveGameOver, winnersPlayersIds);

    public void SendInstantGameStarted(int playersNumberForStartGame, int gameId) => _hubContextQwirkle.Clients.Group(SignalRHub.InstantGameGroupName(playersNumberForStartGame)).SendAsync(INotification.ReceiveInstantGameStarted, playersNumberForStartGame, gameId);
    public void SendInstantGameJoined(int playersNumberForStartGame, string userName) => _hubContextQwirkle.Clients.Group(SignalRHub.InstantGameGroupName(playersNumberForStartGame)).SendAsync(INotification.ReceiveInstantGameJoined, userName);
}