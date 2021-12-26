namespace Qwirkle.Domain.Ports;

public interface INotification
{
    void SendTurnSkipped(int gameId, int playerId);
    void SendPlayerIdTurn(int gameId, int playerId);
    void SendTilesPlayed(int gameId, int playerId, int scoredPoints, List<TileOnBoard> tilesOnBoardPlayed);
    void SendTilesSwapped(int gameId, int playerId);
    void SendGameOver(int gameId, List<int> winnersPlayersIds);
}