namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.NotificationReceiver;

public interface INotificationReceiver
{
    void Initialize(Dictionary<int, string> idPlayerNameDictionary, int playerId, Action stateHaveChange);
    void TilesPlayed(int playerId, Move move);
    void GameOver(int playerId);
    void PlayerIdTurn(int playerId);
    void TurnSkipped(int playerId);
    void TilesSwapped(int playerId);
    void PlayersInGame(HashSet<int> playersIds);
}