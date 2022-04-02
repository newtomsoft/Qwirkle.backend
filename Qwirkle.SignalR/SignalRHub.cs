namespace Qwirkle.SignalR;

public class SignalRHub : Hub
{
    private static readonly Dictionary<int, List<SignalRPlayer>> GameIdWithPlayers = new();
    private static readonly Dictionary<int, HashSet<NotificationUser>> InstantGameWaitingUsers = new();


    public SignalRHub()
    {
        InstantGameWaitingUsers.TryAdd(2, new HashSet<NotificationUser>());
        InstantGameWaitingUsers.TryAdd(3, new HashSet<NotificationUser>());
        InstantGameWaitingUsers.TryAdd(4, new HashSet<NotificationUser>());
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var waitingGamePlayerNumber = InstantGameWaitingUsers.Where(item => item.Value.Count(p => p.ConnectionId == Context.ConnectionId) == 1).Select(item => item.Key).FirstOrDefault();
        if (waitingGamePlayerNumber != 0)
        {
            var user = InstantGameWaitingUsers[waitingGamePlayerNumber].FirstOrDefault(player => player.ConnectionId == Context.ConnectionId);
            InstantGameWaitingUsers[waitingGamePlayerNumber].Remove(user);
            SendUserWaitingInstantGame(waitingGamePlayerNumber);
        }

        var gameId = GameIdWithPlayers.Where(item => item.Value.Count(p => p.ConnectionId == Context.ConnectionId) == 1).Select(item => item.Key).FirstOrDefault();
        if (gameId != 0)
        {
            var player = GameIdWithPlayers[gameId].FirstOrDefault(player => player.ConnectionId == Context.ConnectionId);
            GameIdWithPlayers[gameId].Remove(player);
            SendPlayersInGame(gameId);
        }

        return base.OnDisconnectedAsync(exception);
    }

    public async Task PlayerInGame(int gameId, int playerId)
    {
        GameIdWithPlayers.TryAdd(gameId, new List<SignalRPlayer>());
        var player = new SignalRPlayer(Context.ConnectionId, playerId);
        var playerInGame = GameIdWithPlayers[gameId].Any(p => p.PlayerId == playerId);
        if (!playerInGame)
        {
            GameIdWithPlayers[gameId].Add(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await SendPlayersInGame(gameId);
        }
    }

    public async Task UserWaitingInstantGame(int playerNumberForStartGame, string userName)
    {
        var user = new NotificationUser(Context.ConnectionId, userName);
        var userInGame = InstantGameWaitingUsers[playerNumberForStartGame].Any(u => u.UserName == userName);
        if (!userInGame)
        {
            InstantGameWaitingUsers[playerNumberForStartGame].Add(user);
            await Groups.AddToGroupAsync(Context.ConnectionId, InstantGameGroupName(playerNumberForStartGame));
            await SendUserWaitingInstantGame(playerNumberForStartGame);
        }
    }

    private Task SendPlayersInGame(int gameId) => Clients.Group(gameId.ToString()).SendAsync(INotification.ReceivePlayersInGame, GameIdWithPlayers[gameId].Select(item => item.PlayerId).ToHashSet());
    private Task SendUserWaitingInstantGame(int instantGamePlayerNumber) => Clients.Group(InstantGameGroupName(instantGamePlayerNumber)).SendAsync(INotification.ReceiveUsersWaitingInstantGame, InstantGameWaitingUsers[instantGamePlayerNumber]);
    public static string InstantGameGroupName(int playerNumberForStartGame) => "instantGame" + playerNumberForStartGame;
}
