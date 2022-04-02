namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.Players;

public class PlayersDetail : IPlayersDetail
{
    public PlayerDetail[] All { get; private set; } = Array.Empty<PlayerDetail>();

    public void Initialize(IEnumerable<PlayerDetail> playersDetails, string playerPseudo)
    {
        var players = playersDetails.ToArray();
        var player = players.Single(p => p.Pseudo == playerPseudo);
        var firstPart = players.Where(p => p.GamePosition >= player.GamePosition).OrderBy(p => p.GamePosition).ToList();
        var secondPart = players.Where(p => p.GamePosition < player.GamePosition).OrderBy(p => p.GamePosition);
        firstPart.AddRange(secondPart);
        All = firstPart.ToArray();
    }
    public void OnPlayerTurnChanged(object source, PlayerTurnChangedEventArgs eventArgs)
    {
        All.Single(p => p.IsTurn).IsTurn = false;
        All.Single(p => p.Pseudo == eventArgs.Pseudo).IsTurn = true;
    }

    public void OnPlayerPointsChanged(object source, PlayerPointsChangedEventArgs eventArgs)
    {
        All.Single(p => p.Pseudo == eventArgs.Pseudo).Points += eventArgs.Points;
    }
}