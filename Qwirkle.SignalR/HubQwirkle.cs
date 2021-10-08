using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwikle.SignalR
{
    [Authorize]
    public class HubQwirkle : Hub, IHubQwirkle
    {
        static private Dictionary<string, List<Player>> GameId_Players { get; set; } = new Dictionary<string, List<Player>>();

        public override Task OnConnectedAsync()
        {
            var gameId = GameId_Players.Where(item => item.Value.Count(p => p.ConnectionId == Context.ConnectionId) == 1).Select(item => item.Key).FirstOrDefault();
            if (gameId is not null)
            {
                var player = GameId_Players[gameId].FirstOrDefault(player => player.ConnectionId == Context.ConnectionId);
                player.Connected = true;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var gameId = GameId_Players.Where(item => item.Value.Count(p => p.ConnectionId == Context.ConnectionId) == 1).Select(item => item.Key).FirstOrDefault();
            var player = GameId_Players[gameId].FirstOrDefault(player => player.ConnectionId == Context.ConnectionId);
            player.Connected = false;
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendTilesPlayed(string gameId, List<TileOnBoard> tilesPlayed) => await Clients.Group(gameId).SendAsync("ReceiveTilesPlayed", tilesPlayed);
    }
}
