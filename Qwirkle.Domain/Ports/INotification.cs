namespace Qwirkle.Domain.Ports;

public interface INotification
{
    const string ReceivePlayerIdTurn = "ReceivePlayerIdTurn";
    const string ReceiveTurnSkipped = "ReceiveTurnSkipped";
    const string ReceiveTilesPlayed = "ReceiveTilesPlayed"; //TODO remove
    const string ReceiveTilesPlayedNew = "ReceiveTilesPlayedNew";
    const string ReceiveTilesSwapped = "ReceiveTilesSwapped";
    const string ReceiveGameOver = "ReceiveGameOver";
    const string ReceivePlayersInGame = "ReceivePlayersInGame";

    void SendTurnSkipped(int gameId, int playerId);
    void SendPlayerIdTurn(int gameId, int playerId);
    void SendTilesPlayedOld(int gameId, int playerId, Move move); // TODO remove
    void SendTilesPlayed(int gameId, int playerId, Move move);
    void SendTilesSwapped(int gameId, int playerId);
    void SendGameOver(int gameId, List<int> winnersPlayersIds);


    const string ReceiveInstantGameJoined = "ReceiveInstantGameExpected";
    const string ReceiveInstantGameStarted = "ReceiveInstantGameStarted";
    const string ReceiveUsersWaitingInstantGame = "ReceiveUsersWaitingInstantGame";

    void SendInstantGameStarted(int playersNumberForStartGame, int gameId);
    void SendInstantGameJoined(int playersNumberForStartGame, string userName);
}