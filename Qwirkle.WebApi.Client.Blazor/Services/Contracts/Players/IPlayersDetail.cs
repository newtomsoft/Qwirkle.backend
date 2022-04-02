namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Players;

public interface IPlayersDetail
{
    PlayerDetail[] All { get; }

    void Initialize(IEnumerable<PlayerDetail> playersDetails, string playerPseudo);
    void OnPlayerTurnChanged(object source, PlayerTurnChangedEventArgs eventArgs);
    void OnPlayerPointsChanged(object source, PlayerPointsChangedEventArgs eventArgs);
}