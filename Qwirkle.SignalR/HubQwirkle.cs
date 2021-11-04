using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwirkle.SignalR;

//[Authorize]
public class HubQwirkle : Hub
{
    private static readonly Dictionary<int, List<Player>> GameIdWithPlayers = new();

    public override Task OnConnectedAsync() => base.OnConnectedAsync();

    public override Task OnDisconnectedAsync(Exception exception)
    {
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
        GameIdWithPlayers.TryAdd(gameId, new List<Player>());
        var player = new Player(Context.ConnectionId, playerId);
        var playerInGame = GameIdWithPlayers[gameId].Any(p => p.PlayerId == player.PlayerId);
        if (!playerInGame)
        {
            GameIdWithPlayers[gameId].Add(player);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
            await SendPlayersInGame(gameId);
        }
    }

    private Task SendPlayersInGame(int gameId) => Clients.Group(gameId.ToString()).SendAsync("ReceivePlayersInGame", GameIdWithPlayers[gameId]);

}
