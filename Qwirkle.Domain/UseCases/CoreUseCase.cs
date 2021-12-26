namespace Qwirkle.Domain.UseCases;

public class CoreUseCase
{
    private const int TilesNumberPerPlayer = 6;

    private readonly IRepository _repository;
    private readonly INotification _notification;
    private readonly InfoUseCase _infoUseCase;
    private readonly BotUseCase _botUseCase;

    public Game Game { get; set; }

    public CoreUseCase(IRepository repository, INotification notification, InfoUseCase infoUseCase)
    {
        _repository = repository;
        _notification = notification;
        _infoUseCase = infoUseCase;
        _botUseCase = new BotUseCase(infoUseCase, this);
#warning grosse dette technique

    }

    public List<Player> CreateGame(HashSet<int> usersIds)
    {
        InitializeGame();
        CreatePlayers(usersIds);
        CreateTiles();
        DealTilesToPlayers();
        SelectFirstPlayer();
        return Game.Players;
    }

    public void ResetGame(int gameId) => Game = _repository.GetGame(gameId);

    public ArrangeRackReturn TryArrangeRack(int playerId, IEnumerable<(TileColor color, TileShape shape)> tilesTuple)
    {
        var tilesTupleList = tilesTuple.ToList();
        var player = _infoUseCase.GetPlayer(playerId);
        var tiles = GetTiles(tilesTupleList);
        if (!player.HasTiles(tiles)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        ArrangeRack(player, tilesTupleList);
        return new ArrangeRackReturn { Code = PlayReturnCode.Ok };
    }

    public PlayReturn TryPlayTiles(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        if (!player.IsTurn) return new PlayReturn(player.GameId, PlayReturnCode.NotPlayerTurn, null, null, 0);

        var tilesToPlay = tiles.ToList();

        if (!player.HasTiles(tilesToPlay)) return new PlayReturn(player.GameId, PlayReturnCode.PlayerDoesntHaveThisTile, null, null, 0);

        var game = _repository.GetGame(player.GameId);
        Game = game;
        var playReturn = Play(tilesToPlay, player, game);
        if (playReturn.Code != PlayReturnCode.Ok) return playReturn;

        playReturn = playReturn with { NewRack = PlayTiles(player, tilesToPlay, playReturn.Points) };
        _notification?.SendTilesPlayed(Game.Id, playerId, playReturn.Points, playReturn.TilesPlayed);
        var playerIdTurn = _infoUseCase.GetPlayerIdTurn(Game.Id);
        var nextPlayer = game.Players.First(p => p.Id == playerIdTurn);
        if (nextPlayer.IsBot()) _botUseCase.Play(game, nextPlayer);
        _notification?.SendPlayerIdTurn(Game.Id, playerIdTurn);
        return playReturn;
    }

    public SwapTilesReturn TrySwapTiles(int playerId, IEnumerable<(TileColor color, TileShape shape)> tilesTuple)
    {
        var tilesList = tilesTuple.ToList();
        var player = _infoUseCase.GetPlayer(playerId);
        var tiles = GetTiles(tilesList);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tiles)) return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        var swapTilesReturn = SwapTiles(player, tilesList);
        _notification.SendTilesSwapped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, _infoUseCase.GetPlayerIdTurn(Game.Id));
        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        var skipTurnReturn = player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (skipTurnReturn.Code != PlayReturnCode.Ok) return skipTurnReturn;
        _notification.SendTurnSkipped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, _infoUseCase.GetPlayerIdTurn(Game.Id));
        return skipTurnReturn;
    }

    public PlayReturn TryPlayTilesSimulation(int playerId, IEnumerable<TileOnBoard> tiles)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        var tilesToPlay = tiles.ToList();
        var game = _repository.GetGame(player.GameId);
        return Play(tilesToPlay, player, game, true);
    }

    public PlayReturn Play(List<TileOnBoard> tilesPlayed, Player player, Game game, bool simulationMode = false)
    {
        if (game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, 1);
        if (IsCoordinatesNotFree()) return new PlayReturn(game.Id, PlayReturnCode.NotFree, null, null, 0);
        if (IsBoardNotEmpty() && IsAnyTileIsolated()) return new PlayReturn(game.Id, PlayReturnCode.TileIsolated, null, null, 0);

        var computePointsUseCase = new ComputePointsUseCase();
        var wonPoints = computePointsUseCase.ComputePoints(game, tilesPlayed);
        if (wonPoints == 0) return new PlayReturn(game.Id, PlayReturnCode.TilesDoesntMakedValidRow, null, null, 0);

        if (IsGameFinished())
        {
            const int endGameBonusPoints = 6;
            wonPoints += endGameBonusPoints;
            if (!simulationMode) _repository.SetGameOver(game.Id);
        }
        return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, wonPoints);

        bool IsGameFinished() => IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsBagEmpty() => game.Bag?.Tiles.Count == 0;
        bool IsBoardNotEmpty() => game.Board.Tiles.Count > 0;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !game.Board.IsFreeTile(tile));
    }

    private void InitializeGame() => Game = _repository.CreateGame(DateTime.UtcNow);

    private void DealTilesToPlayers()
    {
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberPerPlayer; i++)
            rackPositions.Add(i);

        foreach (var player in Game.Players)
            _repository.TilesFromBagToPlayer(player, rackPositions);
    }

    private void CreateTiles() => _repository.CreateTiles(Game.Id);

    private void CreatePlayers(HashSet<int> usersIds)
    {
        Game = Game with { Players = new List<Player>() };
        foreach (var userId in usersIds) Game.Players.Add(_repository.CreatePlayer(userId, Game.Id));
        SetPositionsPlayers();
        Game.Players.ForEach(player => _repository.UpdatePlayer(player));
    }

    private void ArrangeRack(Player player, IEnumerable<(TileColor color, TileShape shape)> tilesTuple) => _repository.ArrangeRack(player, tilesTuple);

    private void SetPositionsPlayers()
    {
        Game = Game with { Players = Game.Players.OrderBy(_ => Guid.NewGuid()).ToList() };
        for (var i = 0; i < Game.Players.Count; i++)
            Game.Players[i].GamePosition = (byte)(i + 1);
    }

    private void SelectFirstPlayer()
    {
        var playersWithNumberCanBePlayedTiles = new Dictionary<int, int>();
        Game.Players.ForEach(p => playersWithNumberCanBePlayedTiles[p.Id] = p.TilesNumberCanBePlayedAtGameBeginning());
        var playerIdToPlay = playersWithNumberCanBePlayedTiles.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
        SetPlayerTurn(playerIdToPlay);
    }

    private SkipTurnReturn SkipTurn(Player player)
    {
        ResetGame(player.GameId);
        player.LastTurnSkipped = true;
        if (Game.Bag.Tiles.Count == 0 && Game.Players.Count(p => p.LastTurnSkipped) == Game.Players.Count)
        {
            _repository.UpdatePlayer(player);
            _repository.SetGameOver(player.GameId);
        }
        else
            SetNextPlayerTurnToPlay(player);

        return new SkipTurnReturn { GameId = player.GameId, Code = PlayReturnCode.Ok };
    }

    private SwapTilesReturn SwapTiles(Player player, IEnumerable<(TileColor color, TileShape shape)> tilesTuple)
    {
        var tiles = tilesTuple.ToList();
        ResetGame(player.GameId);
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tiles.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tiles);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = _infoUseCase.GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Player player, IEnumerable<TileOnBoard> tilesToPlay, int points)
    {
        var tilesToPlayList = tilesToPlay.ToList();
        player.LastTurnPoints = points;
        player.Points += points;
        Game.Board.AddTiles(tilesToPlayList);
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlayList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(Game.Id, player.Id, tilesToPlayList);
        return _repository.GetPlayer(player.Id).Rack;
    }

    private void SetNextPlayerTurnToPlay(Player player)
    {
        if (Game.GameOver) return;

        if (Game.Players.Count == 1)
        {
            player.SetTurn(true);
            _repository.UpdatePlayer(player);
        }
        else
        {
            var position = Game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = Game.Players.Count;
            var nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            var nextPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            nextPlayer!.SetTurn(true);
            _repository.UpdatePlayer(player);
            _repository.UpdatePlayer(nextPlayer);
        }
    }

    private void SetPlayerTurn(int playerId)
    {
        _repository.SetPlayerTurn(playerId);
        Game.Players.First(p => p.Id == playerId).SetTurn(true);
    }

    private IEnumerable<Tile> GetTiles(IEnumerable<(TileColor color, TileShape shape)> tilesTuples) => tilesTuples.Select(tileTuple => _repository.GetTile(tileTuple.color, tileTuple.shape));
}