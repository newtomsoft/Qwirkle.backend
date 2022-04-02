namespace Qwirkle.Domain.Ports;

public class NoNotification : INotification
{
    public void SendGameOver(int gameId, List<int> winnersPlayersIds) { }
    public void SendPlayerIdTurn(int gameId, int playerId) { }
    public void SendTilesPlayedOld(int gameId, int playerId, Move move) { }//TODO remove
    public void SendTilesPlayed(int gameId, int playerId, Move move) { }
    public void SendTilesSwapped(int gameId, int playerId) { }
    public void SendTurnSkipped(int gameId, int playerId) { }
    public void SendInstantGameStarted(int playersNumberForStartGame, int gameId) { }
    public void SendInstantGameJoined(int playersNumberForStartGame, string userName) { }
}