namespace Qwirkle.Domain.Services;

public class CoreService
{
    public const int TotalTilesNumber = 108;
    public const int TilesNumberPerPlayer = 6;
    public const int TilesNumberForAQwirkle = 6;
    public const int PointsForAQwirkle = 12;

    private readonly IRepository _repository;
    private readonly INotification _notification;
    private readonly InfoService _infoService;
    private readonly ILogger<CoreService> _logger;
    private readonly BotService _botService;

    public CoreService(IRepository repository, INotification notification, InfoService infoService, UserService userService, ILogger<CoreService> logger)
    {
        _repository = repository;
        _notification = notification;
        _infoService = infoService;
        _logger = logger;
        _botService = new BotService(infoService, this, userService, _logger); //todo dette technique
    }

    public async Task<int> CreateGameAsync(HashSet<int> usersIds)
    {
        _logger?.LogInformation("CreateGame with users {usersIds}", usersIds);
        var game = InitializeEmptyGame();
        CreatePlayers(game, usersIds);
        PutTilesOnBag(game);
        DealTilesToPlayers(game);
        game = SortPlayers(game);
        var gameId = game.Id;
        await _botService.PlayBotsAsync(gameId);
        return gameId;
    }

    public ArrangeRackReturn TryArrangeRack(int playerId, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        var player = _infoService.GetPlayer(playerId);
        if (!player.HasTiles(tilesList)) return new ArrangeRackReturn(ReturnCode.PlayerDoesntHaveThisTile);
        ArrangeRack(player, tilesList);
        return new ArrangeRackReturn(ReturnCode.Ok);
    }

    public PlayReturn TryPlayTiles(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoService.GetPlayer(playerId);
        if (!player.IsTurn) return new PlayReturn(player.GameId, ReturnCode.NotPlayerTurn, Move.Empty, null);

        var tilesToPlay = tiles.ToHashSet();

        if (!player.HasTiles(tilesToPlay)) return new PlayReturn(player.GameId, ReturnCode.PlayerDoesntHaveThisTile, Move.Empty, null);

        var game = _repository.GetGame(player.GameId);
        var playReturn = Play(tilesToPlay, player, game);
        if (playReturn.Code != ReturnCode.Ok) return playReturn;

        playReturn = playReturn with { NewRack = PlayTiles(game, player, tilesToPlay, playReturn.Move.Points) };
        _notification?.SendTilesPlayed(game.Id, playerId, playReturn.Move);

        return playReturn;
    }

    public SwapTilesReturn TrySwapTiles(int playerId, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        var player = _infoService.GetPlayer(playerId);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = player.GameId, Code = ReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tilesList)) return new SwapTilesReturn { GameId = player.GameId, Code = ReturnCode.PlayerDoesntHaveThisTile };
        var game = _repository.GetGame(player.GameId);
        var swapTilesReturn = SwapTiles(game, player, tilesList);
        _notification.SendTilesSwapped(game.Id, playerId);

        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = _infoService.GetPlayer(playerId);
        var skipTurnReturn = player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = player.GameId, Code = ReturnCode.NotPlayerTurn };
        if (skipTurnReturn.Code != ReturnCode.Ok) return skipTurnReturn;

        var game = _repository.GetGame(player.GameId);
        _notification.SendTurnSkipped(game.Id, playerId);

        return skipTurnReturn;
    }

    public PlayReturn TryPlayTilesSimulation(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoService.GetPlayer(playerId);
        var tilesToPlay = tiles.ToHashSet();
        var game = _repository.GetGame(player.GameId);
        return Play(tilesToPlay, player, game, true);
    }

    public PlayReturn Play(HashSet<TileOnBoard> tilesPlayed, Player player, Game game, bool simulationMode = false)
    {
        var orderedTilesPlayed = tilesPlayed.OrderBy(t => t.Coordinates).ToList();
        if (IsCoordinatesNotFree()) return new PlayReturn(game.Id, ReturnCode.NotFree, Move.Empty, Rack.Empty);
        if (!game.IsBoardEmpty() && IsAnyTileIsolated()) return new PlayReturn(game.Id, ReturnCode.TileIsolated, Move.Empty, Rack.Empty);

        var wonPoints = ComputePoints.Compute(game, tilesPlayed);
        if (wonPoints == 0) return new PlayReturn(game.Id, ReturnCode.TilesDoesntMakedValidRow, Move.Empty, Rack.Empty);
        if (game.IsBoardEmpty() && !simulationMode && IsFirstMoveNotCompliant()) return new PlayReturn(game.Id, ReturnCode.NotMostPointsMove, Move.Empty, Rack.Empty);
        if (!IsGameFinished()) return new PlayReturn(game.Id, ReturnCode.Ok, new Move(orderedTilesPlayed, wonPoints), Rack.Empty);

        const int endGameBonusPoints = 6;
        wonPoints += endGameBonusPoints;
        if (!simulationMode) GameOver(game);
        return new PlayReturn(game.Id, ReturnCode.Ok, new Move(orderedTilesPlayed, wonPoints), Rack.Empty);

        bool IsGameFinished() => game.IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !game.Board.IsFreeTile(tile));
        bool IsFirstMoveNotCompliant() => wonPoints != _botService.GetMostPointsToPlay(player, game);
    }

    private void GameOver(Game game) => _repository.SetGameOver(game.Id);

    private Game InitializeEmptyGame() => _repository.CreateGame(DateTime.UtcNow);

    private void DealTilesToPlayers(Game game)
    {
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberPerPlayer; i++) rackPositions.Add(i);
        foreach (var player in game.Players) _repository.TilesFromBagToPlayer(player, rackPositions);
    }

    private void PutTilesOnBag(Game game) => _repository.PutTilesOnBag(game.Id);

    private void CreatePlayers(Game game, HashSet<int> usersIds)
    {
        foreach (var userId in usersIds) game.Players.Add(_repository.CreatePlayer(userId, game.Id));
    }

    private void ArrangeRack(Player player, IEnumerable<Tile> tiles) => _repository.ArrangeRack(player, tiles);

    private Game SortPlayers(Game game)
    {
        var playersWithCanBePlayedTilesNumber = new Dictionary<int, int>();
        game.Players.ForEach(p => playersWithCanBePlayedTilesNumber[p.Id] = p.TilesNumberCanBePlayedAtGameBeginning());

        var playerIdToStart = playersWithCanBePlayedTilesNumber.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
        var playerToStart = game.Players.First(p => p.Id == playerIdToStart);
        var otherPlayers = game.Players.Where(p => p.Id != playerIdToStart).OrderBy(_ => Guid.NewGuid()).ToList();

        var playersOrdered = new List<Player> { playerToStart };
        playersOrdered.AddRange(otherPlayers);
        game = game with { Players = playersOrdered };
        for (byte i = 0; i < game.Players.Count; i++) game.Players[i].GamePosition = i;
        game.Players.ForEach(player => _repository.UpdatePlayer(player));

        SetPlayerTurn(playerToStart);
        return game;
    }

    private SkipTurnReturn SkipTurn(Player player)
    {
        var game = _repository.GetGame(player.GameId);
        player.LastTurnSkipped = true;
        if (game.Bag.TilesNumber == 0 && game.Players.Count(p => p.LastTurnSkipped) == game.Players.Count)
        {
            _repository.UpdatePlayer(player);
            _repository.SetGameOver(player.GameId);
        }
        else
            SetNextPlayerTurnToPlay(game, player);

        return new SkipTurnReturn { GameId = player.GameId, Code = ReturnCode.Ok };
    }

    private SwapTilesReturn SwapTiles(Game game, Player player, IEnumerable<Tile> tiles)
    {
        var tilesList = tiles.ToList();
        SetNextPlayerTurnToPlay(game, player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tilesList);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = ReturnCode.Ok, NewRack = _infoService.GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Game game, Player player, IEnumerable<TileOnBoard> tilesToPlay, int points)
    {
        var tilesToPlayList = tilesToPlay.ToList();
        player.LastTurnPoints = points;
        player.Points += points;
        _repository.UpdatePlayer(player);
        _logger.LogInformation("player {playerId} play {tiles} and get {pointsNumber} points", player.Id, tilesToPlayList.ToLog(), points);
        game.Board.AddTiles(tilesToPlayList);
        SetNextPlayerTurnToPlay(game, player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlayList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(game.Id, player.Id, tilesToPlayList);
        return _repository.GetPlayer(player.Id).Rack;
    }

    private void SetNextPlayerTurnToPlay(Game game, Player player)
    {
        if (game.GameOver) return;

        if (game.Players.Count == 1)
        {
            player.SetTurn(true);
            _repository.UpdatePlayer(player);
        }
        else
        {
            var position = game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = game.Players.Count;
            var nextPlayerPosition = position < playersNumber - 1 ? position + 1 : 0;
            var nextPlayer = game.Players.First(p => p.GamePosition == nextPlayerPosition);
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