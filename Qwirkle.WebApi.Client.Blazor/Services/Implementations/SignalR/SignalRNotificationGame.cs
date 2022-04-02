namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.SignalR;

public class SignalRNotificationGame : INotificationGame
{
    private readonly HubConnection _hubConnection;

    public SignalRNotificationGame(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder().WithUrl(navigationManager.ToAbsoluteUri("/hubGame")).Build();
    }

    public async Task Start() => await _hubConnection.StartAsync();
    public async Task Stop() => await _hubConnection.StopAsync();

    public async Task SendPlayerInGame(int gameId, int playerId) => await _hubConnection.SendAsync(nameof(SignalRHub.PlayerInGame), gameId, playerId);

    public void SubscribeTilesPlayed(Action<int, Move> action) => _hubConnection.On(INotification.ReceiveTilesPlayedNew, action);
    public void SubscribeTilesSwapped(Action<int> action) => _hubConnection.On(INotification.ReceiveTilesSwapped, action);
    public void SubscribeTurnSkipped(Action<int> action) => _hubConnection.On(INotification.ReceiveTurnSkipped, action);
    public void SubscribePlayerIdTurn(Action<int> action) => _hubConnection.On(INotification.ReceivePlayerIdTurn, action);
    public void SubscribeGameOver(Action<int> action) => _hubConnection.On(INotification.ReceiveGameOver, action);
    public void SubscribePlayersInGame(Action<HashSet<int>> action) => _hubConnection.On(INotification.ReceivePlayersInGame, action);

    public async ValueTask DisposeAsync() => await _hubConnection.DisposeAsync();
}