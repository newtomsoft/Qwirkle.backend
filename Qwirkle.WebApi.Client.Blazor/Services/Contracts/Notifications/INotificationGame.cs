namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Notifications;

public interface INotificationGame : IAsyncDisposable
{
    Task Start();
    Task Stop();

    Task SendPlayerInGame(int gameId, int playerId);

    void SubscribePlayerIdTurn(Action<int> action);
    void SubscribeTurnSkipped(Action<int> action);
    void SubscribeTilesPlayed(Action<int, Move> action);
    void SubscribeTilesSwapped(Action<int> action);
    void SubscribeGameOver(Action<int> action);
    void SubscribePlayersInGame(Action<HashSet<int>> action);
}