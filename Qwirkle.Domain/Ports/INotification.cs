namespace Qwirkle.Domain.Ports;

public interface INotification
{
    void SendTurnSkipped(int gameId, int playerId);
    void SendPlayerIdTurn(int gameId, int playerId);
    void SendTilesPlayed(int gameId, int playerId, Move move);
    void SendTilesSwapped(int gameId, int playerId);
    void SendGameOver(int gameId, List<int> winnersPlayersIds);
    void SendInstantGameStarted(int playersNumberForStartGame, int gameId);
    void SendInstantGameExpected(int playersNumberForStartGame, string userName);
}