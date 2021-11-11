using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Ports;

namespace Qwirkle.SignalR;

public class Signal : ISignal
{
    private readonly IHubContext<HubQwirkle> _hubContextQwirkle;

    public Signal(IHubContext<HubQwirkle> hubContextQwirkle)
    {
        _hubContextQwirkle = hubContextQwirkle;
    }

    public void SendTurnSkipped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTurnSkipped", playerId);
    public void SendPlayerIdTurn(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceivePlayerIdTurn", playerId);
    public void SendTilesPlayed(int gameId, int playerId, int scoredPoints, List<TileOnBoard> tilesOnBoardPlayed) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesPlayed", playerId, scoredPoints, tilesOnBoardPlayed);
    public void SendTilesSwapped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesSwapped", playerId);
    public void SendGameOver(int gameId, List<int> winnersPlayersIds) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveGameOver", winnersPlayersIds);
}