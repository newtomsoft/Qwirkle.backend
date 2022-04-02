namespace Qwirkle.WebApi.Client.Blazor.Pages;

public partial class InstantGamePage : IAsyncDisposable
{
    [Inject] private IApiInstantGame ApiInstantGame { get; set; } = default!;
    [Inject] private INotificationInstantGame NotificationInstantGame { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;

    [Parameter] public int PlayersNumber { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    private string _userName = string.Empty;
    private readonly List<string> _playersNamesWaiting = new();

    protected override async Task OnInitializedAsync()
    {
        _userName = AuthenticationStateTask.Result.User.Identity!.Name!;
        await InitializeNotifications();
    }

    private async Task InitializeNotifications()
    {
        NotificationInstantGame.Initialize(NavigationManager.ToAbsoluteUri("/hubGame"));
        NotificationInstantGame.SubscribeInstantGameStarted(InstantGameStarted);
        NotificationInstantGame.SubscribeInstantGameJoined(InstantGameJoinedBy);
        await NotificationInstantGame.Start();
    }

    private async Task JoinInstantGame(int playersNumber)
    {
        PlayersNumber = playersNumber;
        var result = await ApiInstantGame.JoinInstantGame(playersNumber);
        if (result.GameId != 0)
            NavigationManager.NavigateTo($"{PageName.Game}/{result.GameId}");
        else if (result.IsAdded)
        {
            _playersNamesWaiting.AddRange(result.UsersNames.ToList());
            StateHasChanged();
            var otherPlayersNamesWaiting = _playersNamesWaiting.Where(name => name != _userName).ToList();
            if (otherPlayersNamesWaiting.Any()) SnackBar.Add($"{string.Join(", ", otherPlayersNamesWaiting)} waiting with you");
            SnackBar.Add($"Waiting for {playersNumber - _playersNamesWaiting.Count} player(s)");
            await NotificationInstantGame.SendUserWaitingInstantGame(playersNumber, _userName);
        }
        else
        {
            SnackBar.Add($"You're already waiting for the game");
        }
    }

    private void InstantGameStarted(int playersNumberForStartGame, int gameId) => NavigationManager.NavigateTo($"{PageName.Game}/{gameId}");

    private void InstantGameJoinedBy(string userName)
    {
        _playersNamesWaiting.Add(userName);
        SnackBar.Add($"{userName} has join the game");
        SnackBar.Add($"Waiting for {PlayersNumber - _playersNamesWaiting.Count } player(s)");
        StateHasChanged();
    }

    public async ValueTask DisposeAsync() => await NotificationInstantGame.DisposeAsync();
}
