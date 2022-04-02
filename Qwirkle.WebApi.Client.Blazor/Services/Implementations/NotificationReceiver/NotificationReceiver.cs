namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.NotificationReceiver;

public class NotificationReceiver : INotificationReceiver
{
    private event EventHandler<TilesOnBoardPlayedEventArgs> TilesOnBoardPlayed;
    private event EventHandler<PlayerTurnChangedEventArgs> PlayerTurnChanged;
    private event EventHandler<PlayerPointsChangedEventArgs> PlayerPointsChanged;
    private readonly ISnackbar _snackBar;
    private int _playerId;
    private Dictionary<int, string> _playersNames = default!;

    public NotificationReceiver(ISnackbar snackBar, IDragNDropManager dragNDropManager, IAreaManager areaManager, IPlayersDetail playersDetail)
    {
        _snackBar = snackBar;
        TilesOnBoardPlayed += dragNDropManager.OnTilesOnBoardPlayed!;
        TilesOnBoardPlayed += (source, eventArgs) => areaManager.OnTilesOnBoardPlayed(source!, eventArgs);
        PlayerTurnChanged += playersDetail.OnPlayerTurnChanged!;
        PlayerPointsChanged += playersDetail.OnPlayerPointsChanged!;
    }

    public void Initialize(Dictionary<int, string> idPlayerNameDictionary, int playerId, Action stateHaveChange)
    {
        _playersNames = idPlayerNameDictionary;
        _playerId = playerId;
    }


    public void TilesPlayed(int playerId, Move move)
    {
        if (_playerId == playerId) return;

        var (tilesOnBoard, points) = move;
        _snackBar.Add($"{_playersNames[playerId]} has played {tilesOnBoard.Count} tiles and got {points} points");
        OnTilesOnBoardAdded(move.Tiles.ToHashSet());
        OnPlayerPointsChanged(_playersNames[playerId], points);
    }

    public void GameOver(int playerId)
    {
        var playerName = _playerId == playerId ? "You" : _playersNames[playerId];
        _snackBar.Add($"Game is over. {playerName} win");
    }

    public void PlayerIdTurn(int playerId)
    {
        var message = _playerId == playerId ? "It's your turn" : $"It's {_playersNames[playerId]}'s turn";
        _snackBar.Add(message);
        OnPlayerTurnChanged(_playersNames[playerId]);
    }

    public void TurnSkipped(int playerId)
    {
        if (_playerId != playerId) _snackBar.Add($"{_playersNames[playerId]} has skipped his turn");
    }

    public void TilesSwapped(int playerId)
    {
        if (_playerId != playerId) _snackBar.Add($"{_playersNames[playerId]} has swapped tiles");
    }

    public void PlayersInGame(HashSet<int> playersIds)
    {
        playersIds.Remove(_playerId);
        if (playersIds.Count == 1)
        {
            _snackBar.Add($"{_playersNames[playersIds.First()]} is online");
            return;
        }

        var stringBuilder = new StringBuilder();
        foreach (var playerId in playersIds)
        {
            stringBuilder.Append(_playersNames[playerId]);
            stringBuilder.Append(" - ");
        }
        stringBuilder.Remove(stringBuilder.Length - 3, 2);
        stringBuilder.Append("are online");
        _snackBar.Add(stringBuilder.ToString());
    }
    private void OnTilesOnBoardAdded(IEnumerable<TileOnBoard> tilesOnBoard) => TilesOnBoardPlayed.Invoke(this, new TilesOnBoardPlayedEventArgs(tilesOnBoard));
    private void OnPlayerTurnChanged(string playerName) => PlayerTurnChanged.Invoke(this, new PlayerTurnChangedEventArgs(playerName));
    private void OnPlayerPointsChanged(string playerName, int points) => PlayerPointsChanged.Invoke(this, new PlayerPointsChangedEventArgs(playerName, points));
}