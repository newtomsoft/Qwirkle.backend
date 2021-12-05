namespace Qwirkle.Domain.Ports;

public class FakeNotification : INotification
{
    public void SendGameOver(int gameId, List<int> winnersPlayersIds)
    {
        throw new NotImplementedException();
    }

    public void SendPlayerIdTurn(int gameId, int getPlayerIdToPlay)
    {
        throw new NotImplementedException();
    }

    public void SendTilesPlayed(int gameId, int playerId, int scoredPoints, List<TileOnBoard> tilesOnBoardPlayed)
    {
        throw new NotImplementedException();
    }

    public void SendTilesSwapped(int gameId, int playerId)
    {
        throw new NotImplementedException();
    }

    public void SendTurnSkipped(int gameId, int playerId)
    {
        throw new NotImplementedException();
    }
}
