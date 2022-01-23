namespace Qwirkle.Domain.Ports;

public class NoNotification : INotification
{
    public void SendGameOver(int gameId, List<int> winnersPlayersIds)
    {
        // Method intentionally left empty.
    }

    public void SendPlayerIdTurn(int gameId, int playerId)
    {
        // Method intentionally left empty.
    }

    public void SendTilesPlayed(int gameId, int playerId, Move move)
    {
        // Method intentionally left empty.
    }

    public void SendTilesSwapped(int gameId, int playerId)
    {
        // Method intentionally left empty.
    }

    public void SendTurnSkipped(int gameId, int playerId)
    {
        // Method intentionally left empty.
    }

    public void SendInstantGameStarted(int playersNumberForStartGame)
    {
        // Method intentionally left empty.
    }
}