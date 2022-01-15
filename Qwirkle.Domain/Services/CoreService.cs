namespace Qwirkle.Domain.Services;

public class CoreService
{
    public const int TilesNumberPerPlayer = 6;

    private readonly IRepository _repository;
    private readonly INotification _notification;
    private readonly InfoService _infoService;
    private readonly ILogger<CoreService> _logger;
    private readonly BotService _botService;

    private Game _game;

    public CoreService(IRepository repository, INotification notification, InfoService infoService, ILogger<CoreService> logger)
    {
        _repository = repository;
        _notification = notification;
        _infoService = infoService;
        _logger = logger;
        _botService = new BotService(infoService, this, _logger);
        //todo dette technique à rembourser
    }

    public List<Player> CreateGame(HashSet<int> usersIds)
    {
        _logger.LogInformation("{applicationEvent} at {dateTime}", "CreateGame", DateTime.Now);
        InitializeEmptyGame();
        CreatePlayers(usersIds);
        PutTilesOnBag();
        DealTilesToPlayers();
        SortPlayers();
        return _game.Players;
    }

    public void ResetGame(int gameId) => _game = _repository.GetGame(gameId);

    public ArrangeRackReturn TryArrangeRack(int playerId, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        var player = _infoService.GetPlayer(playerId);
        if (!player.HasTiles(tilesList)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        ArrangeRack(player, tilesList);
        return new ArrangeRackReturn { Code = PlayReturnCode.Ok };
    }

    public PlayReturn TryPlayTiles(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoService.GetPlayer(playerId);
        if (!player.IsTurn) return new PlayReturn(player.GameId, PlayReturnCode.NotPlayerTurn, null, null, 0);

        var tilesToPlay = tiles.ToList();

        if (!player.HasTiles(tilesToPlay)) return new PlayReturn(player.GameId, PlayReturnCode.PlayerDoesntHaveThisTile, null, null, 0);

        var game = _repository.GetGame(player.GameId);
        _game = game;
        var playReturn = Play(tilesToPlay, player, game);
        if (playReturn.Code != PlayReturnCode.Ok) return playReturn;

        playReturn = playReturn with { NewRack = PlayTiles(player, tilesToPlay, playReturn.Points) };
        _notification?.SendTilesPlayed(game.Id, playerId, playReturn.Points, playReturn.TilesPlayed);

        return playReturn;
    }
    
    public SwapTilesReturn TrySwapTiles(int playerId, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        var player = _infoService.GetPlayer(playerId);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tilesList)) return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        var game = _repository.GetGame(player.GameId);

        var swapTilesReturn = SwapTiles(player, tilesList);
        _notification.SendTilesSwapped(game.Id, playerId);

        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = _infoService.GetPlayer(playerId);
        var skipTurnReturn = player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = player.GameId, Code = PlayReturnCode.NotPlayerTurn };
        if (skipTurnReturn.Code != PlayReturnCode.Ok) return skipTurnReturn;

        var game = _repository.GetGame(player.GameId);
        _notification.SendTurnSkipped(game.Id, playerId);

        return skipTurnReturn;
    }

    public PlayReturn TryPlayTilesSimulation(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoService.GetPlayer(playerId);
        var tilesToPlay = tiles.ToList();
        var game = _repository.GetGame(player.GameId);
        return Play(tilesToPlay, player, game, true);
    }

    public PlayReturn Play(List<TileOnBoard> tilesPlayed, Player player, Game game, bool simulationMode = false)
    {
        if (IsCoordinatesNotFree()) return new PlayReturn(game.Id, PlayReturnCode.NotFree, null, null, 0);
        if (!game.IsBoardEmpty() && IsAnyTileIsolated()) return new PlayReturn(game.Id, PlayReturnCode.TileIsolated, null, null, 0);

        var wonPoints = ComputePoints.Compute(game, tilesPlayed);
        if (wonPoints == 0) return new PlayReturn(game.Id, PlayReturnCode.TilesDoesntMakedValidRow, null, null, 0);
        if (game.IsBoardEmpty() && !simulationMode && IsFirstMoveNotCompliant()) return new PlayReturn(game.Id, PlayReturnCode.NotMostPointsMove, null, null, 0);
        if (!IsGameFinished()) return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, wonPoints);

        const int endGameBonusPoints = 6;
        wonPoints += endGameBonusPoints;
        if (!simulationMode) GameOver();
        return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, wonPoints);

        bool IsGameFinished() => game.IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !game.Board.IsFreeTile(tile));
        bool IsFirstMoveNotCompliant() => wonPoints != _botService.GetMostPointsToPlay(player, game);
    }

    private void GameOver()
    {
        _game = _game with { GameOver = true };
        _repository.SetGameOver(_game.Id);
    }

    private void InitializeEmptyGame() => _game = _repository.CreateGame(DateTime.UtcNow);

    private void DealTilesToPlayers()
    {
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberPerPlayer; i++) rackPositions.Add(i);
        foreach (var player in _game.Players) _repository.TilesFromBagToPlayer(player, rackPositions);
    }

    private void PutTilesOnBag() => _repository.PutTilesOnBag(_game.Id);

    private void CreatePlayers(HashSet<int> usersIds)
    {
        foreach (var userId in usersIds) _game.Players.Add(_repository.CreatePlayer(userId, _game.Id));
    }

    private void ArrangeRack(Player player, IEnumerable<Tile> tiles) => _repository.ArrangeRack(player, tiles);

    private void SortPlayers()
    {
        var playersWithCanBePlayedTilesNumber = new Dictionary<int, int>();
        _game.Players.ForEach(p => playersWithCanBePlayedTilesNumber[p.Id] = p.TilesNumberCanBePlayedAtGameBeginning());

        var playerIdToStart = playersWithCanBePlayedTilesNumber.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
        var playerToStart = _game.Players.First(p => p.Id == playerIdToStart);
        var otherPlayers = _game.Players.Where(p => p.Id != playerIdToStart).OrderBy(_ => Guid.NewGuid()).ToList();

        var playersOrdered = new List<Player> {playerToStart};
        playersOrdered.AddRange(otherPlayers);
        _game = _game with { Players = playersOrdered };
        for (byte i = 0; i < _game.Players.Count; i++) _game.Players[i].GamePosition = i;
        _game.Players.ForEach(player => _repository.UpdatePlayer(player));

        SetPlayerTurn(playerToStart);
    }

    private SkipTurnReturn SkipTurn(Player player)
    {
        ResetGame(player.GameId);
        player.LastTurnSkipped = true;
        if (_game.Bag.Tiles.Count == 0 && _game.Players.Count(p => p.LastTurnSkipped) == _game.Players.Count)
        {
            _repository.UpdatePlayer(player);
            _repository.SetGameOver(player.GameId);
        }
        else
            SetNextPlayerTurnToPlay(player);

        return new SkipTurnReturn { GameId = player.GameId, Code = PlayReturnCode.Ok };
    }

    private SwapTilesReturn SwapTiles(Player player, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        ResetGame(player.GameId);
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tilesList);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = _infoService.GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Player player, IEnumerable<TileOnBoard> tilesToPlay, int points)
    {
        var tilesToPlayList = tilesToPlay.ToList();
        player.LastTurnPoints = points;
        player.Points += points;
        _repository.UpdatePlayer(player);
        _logger.LogInformation($"player {player.Id} play {tilesToPlayList.ToLog()} and get {points} points");
        _game.Board.AddTiles(tilesToPlayList);
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlayList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(_game.Id, player.Id, tilesToPlayList);
        return _repository.GetPlayer(player.Id).Rack;
    }

    private void SetNextPlayerTurnToPlay(Player player)
    {
        if (_game.GameOver) return;

        if (_game.Players.Count == 1)
        {
            player.SetTurn(true);
            _repository.UpdatePlayer(player);
        }
        else
        {
            var position = _game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = _game.Players.Count;
            var nextPlayerPosition = position < playersNumber - 1 ? position + 1 : 0;
            var nextPlayer = _game.Players.First(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            _repository.UpdatePlayer(player);
            nextPlayer.SetTurn(true);
            _repository.UpdatePlayer(nextPlayer);
        }
    }

    private void SetPlayerTurn(Player player)
    {
        player.SetTurn(true);
        _repository.SetPlayerTurn(player.Id);
    }
}