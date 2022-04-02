namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.SignalR;

public class SignalRNotificationInstantGame : INotificationInstantGame
{
    private HubConnection? _hubConnection;

    public void Initialize(Uri hubUri)
    {
        _hubConnection ??= new HubConnectionBuilder().WithUrl(hubUri).Build();
    }
    public async Task Start() => await _hubConnection!.StartAsync();

    public async Task SendUserWaitingInstantGame(int playersNumber, string userName) => await _hubConnection!.SendAsync(nameof(SignalRHub.UserWaitingInstantGame), playersNumber, userName);
    public void SubscribeInstantGameStarted(Action<int, int> receiveInstantGameStarted) => _hubConnection!.On(INotification.ReceiveInstantGameStarted, receiveInstantGameStarted);
    public void SubscribeInstantGameJoined(Action<string> receiveInstantJoined) => _hubConnection!.On(INotification.ReceiveInstantGameJoined, receiveInstantJoined);


    public async ValueTask DisposeAsync()
    {
        await _hubConnection!.DisposeAsync();
    }
}