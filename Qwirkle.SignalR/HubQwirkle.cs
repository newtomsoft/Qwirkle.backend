using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwirkle.SignalR;

//[Authorize]
public class HubQwirkle : Hub
{
    static private Dictionary<int, List<Player>> GameId_Players { get; set; } = new Dictionary<int, List<Player>>();

    public override Task OnConnectedAsync() => base.OnConnectedAsync();

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var gameId = GameId_Players.Where(item => item.Value.Count(p => p.ConnectionId == Context.ConnectionId) == 1).Select(item => item.Key).FirstOrDefault();
        if (gameId != 0)
        {
            var player = GameId_Players[gameId].FirstOrDefault(player => player.ConnectionId == Context.ConnectionId);
            GameId_Players[gameId].Remove(player);
            SendPlayersInGame(gameId);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task PlayerInGame(int gameId, int playerId)
    {
        GameId_Players.TryAdd(gameId, new List<Player>());
        var player = new Player(Context.ConnectionId, playerId);
        var playerInGame = GameId_Players[gameId].Where(p => p.PlayerId == player.PlayerId).Any();
        if (!playerInGame)
        {
            GameId_Players[gameId].Add(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await SendPlayersInGame(gameId);
        }
    }

    private Task SendPlayersInGame(int gameId) => Clients.Group(gameId.ToString()).SendAsync("ReceivePlayersInGame", GameId_Players[gameId]);

}
