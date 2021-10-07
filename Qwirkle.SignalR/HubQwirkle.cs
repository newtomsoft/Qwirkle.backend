using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qwikle.SignalR.Hubs
{
    [Authorize]
    public class HubQwirkle : Hub, IHubQwirkle
    {
        static public List<int> LoggedPlayersId { get; set; }
        static private Dictionary<string, List<int>> GameGuid_PlayersId { get; set; }
        static private Dictionary<string, string> ContextId_GameGuid { get; set; }

        public override Task OnConnectedAsync()
        {
            (LoggedPlayersId ??= new List<int>()).Add(int.Parse(Context.UserIdentifier));
            Clients.All.SendAsync("ReceivePlayersLogged", LoggedPlayersId.ToHashSet());
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            int playerId = int.Parse(Context.UserIdentifier);
            LoggedPlayersId.Remove(playerId);
            Clients.All.SendAsync("ReceivePlayersLogged", LoggedPlayersId.ToHashSet());

            if (GameGuid_PlayersId is not null)
            {
                string guid = ContextId_GameGuid[Context.ConnectionId];
                GameGuid_PlayersId[guid].Remove(playerId);
                Clients.Group(guid).SendAsync("ReceivePlayersInGame", GameGuid_PlayersId[guid].ToHashSet());

            }
            if (ContextId_GameGuid is not null)
                ContextId_GameGuid.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Envoie l'information que le joueur a une page ouverte sur le jeu
        /// </summary>
        /// <param name="guid">Guid du jeu</param>
        /// <returns></returns>
        public async Task SendAddToGame(string guid)
        {
            GameGuid_PlayersId ??= new Dictionary<string, List<int>>();
            GameGuid_PlayersId.TryAdd(guid, new List<int>());
            GameGuid_PlayersId[guid].Add(int.Parse(Context.UserIdentifier));

            ContextId_GameGuid ??= new Dictionary<string, string>();
            ContextId_GameGuid[Context.ConnectionId] = guid;

            await Groups.AddToGroupAsync(Context.ConnectionId, guid);
            await Clients.Group(guid).SendAsync("ReceivePlayersInGame", GameGuid_PlayersId[guid].ToHashSet());
        }

        public async Task SendTilesPlayed(string guid, List<string> playersId, List<Tile> tilesPlayed) => await Clients.Users(playersId).SendAsync("ReceiveTilesPlayed", guid, tilesPlayed);
    }
}
