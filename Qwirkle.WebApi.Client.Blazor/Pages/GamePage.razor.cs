namespace Qwirkle.WebApi.Client.Blazor.Pages;

public partial class GamePage : IAsyncDisposable
{
    [Parameter] public int GameId { get; set; }
    [Inject] private IApiAction ApiAction { get; set; } = default!;
    [Inject] private IApiGame ApiGame { get; set; } = default!;
    [Inject] private IApiPlayer ApiPlayer { get; set; } = default!;
    [Inject] private INotificationGame NotificationGame { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private INotificationReceiver NotificationReceiver { get; set; } = default!;
    [Inject] private IAreaManager AreaManager { get; set; } = default!;
    [Inject] private IDragNDropManager DragNDropManager { get; set; } = default!;
    [Inject] private IPlayersDetail PlayersDetail { get; set; } = default!;

    private string _actionResult = string.Empty;
    private Game _game = default!;
    private Player _player = default!;
    private bool _isInitialized;

    private event EventHandler<TilesOnBoardPlayedEventArgs> TilesOnBoardPlayed = default!;
    private event EventHandler<TilesOnRackChangedEventArgs> TilesOnRackChanged = default!;
    private event EventHandler<PlayerPointsChangedEventArgs> PlayerPointsChanged = default!;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (GameId <= 0) return;

        _game = await ApiGame.GetGame(GameId);
        _player = await ApiPlayer.GetByGameId(GameId);
        if (_game.GameOver) SnackBar.Add("Game is over");
        PlayersDetail.Initialize(_game.Players.Select(p => new PlayerDetail(p)), _player.Pseudo);
        DragNDropManager.Initialize();
        await InitializeNotifications();
        TilesOnBoardPlayed += DragNDropManager.OnTilesOnBoardPlayed!;
        TilesOnBoardPlayed += (source, eventArgs) => AreaManager.OnTilesOnBoardPlayed(source!, eventArgs);
        TilesOnRackChanged += DragNDropManager.OnTilesOnRackChanged!;
        PlayerPointsChanged += PlayersDetail.OnPlayerPointsChanged!;
        OnTilesOnBoardPlayed(_game.Board.Tiles);
        OnTilesOnRackChanged(_player.Rack.Tiles);

        await NotificationGame.SendPlayerInGame(GameId, _player.Id);
        _isInitialized = true;
    }

    protected override Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }

    private async Task InitializeNotifications()
    {
        var idPlayerNameDictionary = _game.Players.ToDictionary(player => player.Id, player => player.Pseudo);
        NotificationReceiver.Initialize(idPlayerNameDictionary, _player.Id, StateHasChanged);
        NotificationGame.SubscribePlayersInGame(NotificationReceiver.PlayersInGame);
        NotificationGame.SubscribeTilesPlayed(NotificationReceiver.TilesPlayed);
        NotificationGame.SubscribeTilesSwapped(NotificationReceiver.TilesSwapped);
        NotificationGame.SubscribeTurnSkipped(NotificationReceiver.TurnSkipped);
        NotificationGame.SubscribePlayerIdTurn(NotificationReceiver.PlayerIdTurn);
        NotificationGame.SubscribeGameOver(NotificationReceiver.GameOver);
        await NotificationGame.Start();
    }

    private async Task PlayTiles()
    {
        await ArrangeRack();

        var tilesModel = DragNDropManager.TilesDroppedOnBoard.Select(t => new PlayTileModel(GameId, t.Tile, t.Coordinate)).ToList();
        if (!tilesModel.Any())
        {
            _actionResult = "please move some tiles into board";
            SnackBar.Add(_actionResult);
            return;
        }
        var playReturn = await ApiAction.PlayTiles(tilesModel);
        _actionResult = playReturn.Code.ToString();
        if (playReturn is { Code: ReturnCode.Ok })
        {
            _player.Rack = playReturn.NewRack;
            OnTilesOnRackChanged(playReturn.NewRack.Tiles);
            var tilesAdded = tilesModel.Select(t => t.ToTileOnBoard()).ToList();
            _game.Board.AddTiles(tilesAdded);
            OnTilesOnBoardPlayed(tilesAdded);
            OnPlayerPointsChanged(_player.Pseudo, playReturn.Move.Points);
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            SnackBar.Add($"{playReturn.Code.ToDisplay()}");
        }
    }

    private async Task SwapTiles()
    {
        await ArrangeRack();

        var tilesModel = DragNDropManager.TilesDroppedOnBag.Select(t => new SwapTileModel(GameId, t.Tile, t.RackPosition)).ToList();
        if (!tilesModel.Any())
        {
            _actionResult = "please move some tiles into bag";
            SnackBar.Add(_actionResult);
            return;
        }
        var swapTilesReturn = await ApiAction.SwapTiles(tilesModel);
        _actionResult = swapTilesReturn.Code.ToString();
        if (swapTilesReturn is { Code: ReturnCode.Ok })
        {
            DragNDropManager.TilesDroppedOnBag.Clear();
            _player.Rack = swapTilesReturn.NewRack;
            OnTilesOnRackChanged(_player.Rack.Tiles);
            await InvokeAsync(StateHasChanged);
        }
        else
        {
            SnackBar.Add($"{swapTilesReturn.Code.ToDisplay()}");
        }
    }

    private async Task SkipTurn()
    {
        var skipTurnModel = new SkipTurnModel(GameId);
        await ArrangeRack();
        var skipTurnReturn = await ApiAction.SkipTurn(skipTurnModel);
        _actionResult = skipTurnReturn.Code.ToString();
        if (skipTurnReturn is not { Code: ReturnCode.Ok })
            SnackBar.Add($"{skipTurnReturn.Code.ToDisplay()}", Severity.Error);
    }

    public async ValueTask DisposeAsync() => await NotificationGame.Stop();

    private void OnTilesOnRackChanged(List<TileOnRack> tilesOnRack) => TilesOnRackChanged.Invoke(this, new TilesOnRackChangedEventArgs(tilesOnRack));
    private void OnTilesOnBoardPlayed(IEnumerable<TileOnBoard> tilesOnBoard) => TilesOnBoardPlayed.Invoke(this, new TilesOnBoardPlayedEventArgs(tilesOnBoard));
    private void OnPlayerPointsChanged(string playerName, int points) => PlayerPointsChanged.Invoke(this, new PlayerPointsChangedEventArgs(playerName, points));


    private static string ImagePath(Tile tile) => $"img/{tile.Color}{tile.Shape}.svg";

    private async Task ArrangeRack()
    {
        var tilesArrangedModel = DragNDropManager.AllPlayerTiles.Select(t => new ArrangeTileModel(GameId, t.Tile, t.RackPosition)).ToList();
        await ApiAction.ArrangeRack(tilesArrangedModel);
    }
}