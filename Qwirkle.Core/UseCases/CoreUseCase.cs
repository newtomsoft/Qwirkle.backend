namespace Qwirkle.Core.UseCases;

public class CoreUseCase
{
    private const int TilesNumberPerPlayer = 6;
    private const int TilesNumberForAQwirkle = 6;
    private const int PointsForAQwirkle = 12;
    private readonly IRepository _repository;
    private readonly INotification _notification;

    public Game Game { get; set; }

    public CoreUseCase(IRepository repository, INotification notification)
    {
        _repository = repository;
        _notification = notification;
    }

    public int GetUserId(int playerId) => _repository.GetUserId(playerId);

    public List<Player> CreateGame(List<int> usersIds)
    {
        Game = _repository.CreateGame(DateTime.UtcNow);
        CreatePlayers(usersIds);
        CreateTiles();
        DealTilesToPlayers();
        SelectFirstPlayer();
        return Game.Players;
    }

    public ArrangeRackReturn TryArrangeRack(int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesToArrangeTuple)
    {
        var player = GetPlayer(playerId);
        var tilesIds = GetTiles(tilesToArrangeTuple).Select(tiles => tiles.Id).ToList();

        if (!player.HasTiles(tilesIds)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDoesntHaveThisTile };

        var tilesToArrange = GetPlayerTiles(playerId, tilesIds);
        ArrangeRack(player, tilesToArrange);
        return new ArrangeRackReturn { Code = PlayReturnCode.Ok };
    }

    public PlayReturn TryPlayTiles(int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        var player = GetPlayer(playerId);
        if (!player.IsTurn) return new PlayReturn { Code = PlayReturnCode.NotPlayerTurn, GameId = player.GameId };

        var tilesToPlay = GetTiles(tilesTupleToPlay);
        var tilesIds = tilesToPlay.Select(tiles => tiles.Id).ToList();

        InitializeGame(player.GameId);
        if (!player.HasTiles(tilesIds)) return new PlayReturn { Code = PlayReturnCode.PlayerDoesntHaveThisTile, GameId = Game.Id };

        var playReturn = GetPlayReturn(tilesToPlay, player);
        if (playReturn.Code != PlayReturnCode.Ok) return playReturn;

        playReturn.NewRack = PlayTiles(player, tilesToPlay, playReturn.Points);
        _notification?.SendTilesPlayed(Game.Id, playerId, playReturn.Points, playReturn.TilesPlayed);
        _notification?.SendPlayerIdTurn(Game.Id, GetPlayerIdTurn(Game.Id));
        return playReturn;
    }

    private PlayReturn TryPlayTilesSimulationIA(Player player, List<TileOnBoard> tilesToPlay) => GetPlayReturn(tilesToPlay, player, true);

    public PlayReturn TryPlayTilesSimulation(int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        var player = GetPlayer(playerId);
        var tilesToPlay = GetTiles(tilesTupleToPlay);
        InitializeGame(player.GameId);
        return GetPlayReturn(tilesToPlay, player, true);
    }

    public SwapTilesReturn TrySwapTiles(int playerId, IEnumerable<int> tilesIds)
    {
        var tilesIdsArray = tilesIds.ToArray();
        var player = GetPlayer(playerId);
        InitializeGame(player.GameId);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tilesIdsArray)) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        var tilesToSwap = GetPlayerTiles(playerId, tilesIdsArray);
        var swapTilesReturn = SwapTiles(player, tilesToSwap);
        _notification.SendTilesSwapped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, GetPlayerIdTurn(Game.Id));
        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = GetPlayer(playerId);
        InitializeGame(player.GameId);
        var skipTurnReturn = player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (skipTurnReturn.Code != PlayReturnCode.Ok) return skipTurnReturn;
        _notification.SendTurnSkipped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, GetPlayerIdTurn(Game.Id));
        return skipTurnReturn;
    }

    private void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange) => _repository.ArrangeRack(player, tilesToArrange);

    private void DealTilesToPlayers()
    {
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberPerPlayer; i++)
            rackPositions.Add(i);

        foreach (var player in Game.Players)
            _repository.TilesFromBagToPlayer(player, rackPositions);
    }

    private void CreateTiles() => _repository.CreateTiles(Game.Id);

    private void CreatePlayers(List<int> usersIds)
    {
        Game.Players = new List<Player>();
        usersIds.ForEach(userId => Game.Players.Add(_repository.CreatePlayer(userId, Game.Id)));
        SetPositionsPlayers();
        Game.Players.ForEach(player => _repository.UpdatePlayer(player));
    }

    private void SetPositionsPlayers()
    {
        Game.Players = Game.Players.OrderBy(_ => Guid.NewGuid()).ToList();
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

    public PlayReturn GetPlayReturn(List<TileOnBoard> tilesPlayed, Player player, bool simulationMode = false)
    {
        if (Game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn { Code = PlayReturnCode.Ok, Points = 1, TilesPlayed = tilesPlayed, GameId = Game.Id, };
        if (IsBoardNotEmpty() && IsAnyTileIsolated()) return new PlayReturn { Code = PlayReturnCode.TileIsolated, Points = 0, GameId = Game.Id };

        var wonPoints = ComputePoints(tilesPlayed);
        if (wonPoints == 0) return new PlayReturn { Code = PlayReturnCode.TilesDoesntMakedValidRow, GameId = Game.Id };

        if (IsGameFinished())
        {
            const int endGameBonusPoints = 6;
            wonPoints += endGameBonusPoints;
            if (!simulationMode) _repository.SetGameOver(Game.Id);
        }
        return new PlayReturn { Code = PlayReturnCode.Ok, Points = wonPoints, GameId = Game.Id, TilesPlayed = tilesPlayed };

        bool IsGameFinished() => IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsBagEmpty() => Game.Bag?.Tiles.Count == 0;
        bool IsBoardNotEmpty() => Game.Board.Tiles.Count > 0;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => Game.Board.IsIsolatedTile(tile));
    }

    public Player GetPlayer(int playerId) => _repository.GetPlayer(playerId);
    public Player GetPlayer(int gameId, int userId) => _repository.GetPlayer(gameId, userId);
    public string GetPlayerNameTurn(int gameId) => _repository.GetPlayerNameTurn(gameId);
    public int GetPlayerIdTurn(int gameId) => _repository.GetPlayerIdToPlay(gameId);
    public List<int> GetGamesIdsContainingPlayers() => _repository.GetGamesIdsContainingPlayers();
    public List<int> GetAllUsersId() => _repository.GetAllUsersId();
    public List<int> GetUserGames(int userId) => _repository.GetUserGames(userId);
    public Game GetGameForSuperUser(int gameId) => _repository.GetGame(gameId);
    public Game GetGameWithTilesOnlyForAuthenticatedUser(int gameId, int userId)
    {
        var game = _repository.GetGame(gameId);
        var isUserInGame = game.Players.Any(p => p.UserId == userId);
        if (!isUserInGame) return null;
        var otherPlayers = game.Players.Where(p => p.UserId != userId);
        foreach (var player in otherPlayers) player.Rack = new Rack(null);
        return game;
    }

    public List<int> GetWinnersPlayersId(int gameId)
    {
        if (!_repository.IsGameOver(gameId)) return null;
        var winnersPlayersIds = _repository.GetLeadersPlayersId(gameId);
        _notification.SendGameOver(gameId, winnersPlayersIds);
        return winnersPlayersIds;
    }

    public PlayReturn[] GetDoableMoves2(int gameId, int userId)
    {
        var watch = new Stopwatch();
        watch.Start();
        var player = _repository.GetPlayer(gameId, userId);
        var board = _repository.GetGame(gameId).Board;
        InitializeGame(player.GameId);
        var rack = player.Rack;
        var allFunc = new List<Func<PlayReturn>>();

        foreach (var coordinates in board.GetAdjoiningCoordinatesToTiles())
            foreach (var tile in rack.Tiles)
            {
                var tilesOnBoard = new List<TileOnBoard> { TileOnBoard.From(tile, coordinates) };
                allFunc.Add(() => TryPlayTilesSimulationIA(player, tilesOnBoard));
            }
        var playReturns = new PlayReturn[allFunc.Count];
        Parallel.For(0, allFunc.Count, i =>
        {
            playReturns[i] = allFunc[i]();
        });

        playReturns = playReturns.OrderByDescending(p => p.Points).Where(p => p.Points > 0).ToArray();
        //we have all possible moves with 1 tile !

        var watchElapsedMilliseconds = watch.ElapsedMilliseconds;
        watch.Restart();
        return playReturns;
    }

    public List<PlayReturn> GetDoableMoves(int gameId, int userId)
    {
        var watch = new Stopwatch();
        watch.Start();
        var player = _repository.GetPlayer(gameId, userId);
        var board = _repository.GetGame(gameId).Board;
        InitializeGame(player.GameId);
        var rack = player.Rack;
        var allPlayReturns = new List<PlayReturn>();
        var playReturnsWith1Tile = new List<PlayReturn>();

        var boardAdjoiningCoordinates = board.GetAdjoiningCoordinatesToTiles();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            foreach (var tile in rack.Tiles)
            {
                var playReturn = TryPlayTilesSimulationIA(player, new List<TileOnBoard> { TileOnBoard.From(tile, coordinates) });
                if (playReturn.Code == PlayReturnCode.Ok) playReturnsWith1Tile.Add(playReturn);
            }
        }
        playReturnsWith1Tile = playReturnsWith1Tile.OrderByDescending(p => p.Points).ToList();

        var watchElapsedMilliseconds = watch.ElapsedMilliseconds;

        var playReturnsWith2Tiles = new List<PlayReturn>();
        foreach (var playReturn in playReturnsWith1Tile)
        {
            var tilePlayed = playReturn.TilesPlayed[0];
            int tilePlayedX = tilePlayed.Coordinates.X;
            int tilePlayedY = tilePlayed.Coordinates.Y;
            var currentRackTiles = rack.Tiles.Where(t => t.Id != tilePlayed.Id).ToList();

            var boardAdjoiningCoordinatesLine = boardAdjoiningCoordinates.Where(c => c.Y == tilePlayedY).Select(c => (int)c.X).ToList();
            if (tilePlayedX == boardAdjoiningCoordinatesLine.Max()) boardAdjoiningCoordinatesLine.Add(tilePlayedX + 1);
            if (tilePlayedX == boardAdjoiningCoordinatesLine.Min()) boardAdjoiningCoordinatesLine.Add(tilePlayedX - 1);
            boardAdjoiningCoordinatesLine.Remove(tilePlayedX);
            foreach (var x in boardAdjoiningCoordinatesLine)
            {
                foreach (var tile in currentRackTiles)
                {
                    var playReturn2 = TryPlayTilesSimulationIA(player,
                        new List<TileOnBoard> { tilePlayed, TileOnBoard.From(tile, Coordinates.From(x, tilePlayedY)) });
                    if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith2Tiles.Add(playReturn2);
                }
            }
            var boardAdjoiningCoordinatesColumn = boardAdjoiningCoordinates.Where(c => c.X == tilePlayedX).Select(c => (int)c.Y).ToList();
            if (tilePlayedY == boardAdjoiningCoordinatesColumn.Max()) boardAdjoiningCoordinatesColumn.Add(tilePlayedY + 1);
            if (tilePlayedY == boardAdjoiningCoordinatesColumn.Min()) boardAdjoiningCoordinatesColumn.Add(tilePlayedY - 1);
            boardAdjoiningCoordinatesLine.Remove(tilePlayedY);
            foreach (var y in boardAdjoiningCoordinatesColumn)
            {
                foreach (var tile in currentRackTiles)
                {
                    var playReturn2 = TryPlayTilesSimulationIA(player,
                        new List<TileOnBoard> { tilePlayed, TileOnBoard.From(tile, Coordinates.From(tilePlayedX, y)) });
                    if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith2Tiles.Add(playReturn2);
                }
            }
        }

        //we have all possible moves with 2 tiles !
        allPlayReturns.AddRange(playReturnsWith2Tiles);
        allPlayReturns.AddRange(playReturnsWith1Tile);

        watch.Restart();
        return allPlayReturns;
    }

    private void InitializeGame(int gameId) => Game = _repository.GetGame(gameId);

    private SkipTurnReturn SkipTurn(Player player)
    {
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

    private SwapTilesReturn SwapTiles(Player player, List<TileOnPlayer> tilesToSwap)
    {
        var positionsInRack = PositionsInRack(tilesToSwap);
        SetNextPlayerTurnToPlay(player);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tilesToSwap);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Player player, List<TileOnBoard> tilesToPlay, int points)
    {
        player.LastTurnPoints = points;
        player.Points += points;
        Game.Board.Tiles.AddRange(tilesToPlay);
        SetNextPlayerTurnToPlay(player);

        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlay.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(Game.Id, player.Id, tilesToPlay);
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

    private int ComputePoints(List<TileOnBoard> tiles)
    {
        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
            return 0;

        var totalPoints = 0;
        int points;
        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
        {
            if ((points = ComputePointsInLine(tiles)) is 0) return 0;
            if (points is not 1) totalPoints += points;
            if (tiles.Count > 1)
            {
                foreach (var tile in tiles)
                {
                    if ((points = ComputePointsInColumn(new List<TileOnBoard> { tile })) is 0) return 0;
                    if (points is not 1) totalPoints += points;
                }
            }
        }

        if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count) return totalPoints;
        if ((points = ComputePointsInColumn(tiles)) == 0) return 0;
        if (points != 1) totalPoints += points;
        if (tiles.Count <= 1) return totalPoints;
        foreach (var tile in tiles)
        {
            if ((points = ComputePointsInLine(new List<TileOnBoard> { tile })) == 0) return 0;
            if (points != 1) totalPoints += points;
        }
        return totalPoints;
    }

    private int ComputePointsInLine(IReadOnlyList<TileOnBoard> tiles)
    {
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
        var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesRight = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
        var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
        allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

        var tilesLeft = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
        var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
        allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private int ComputePointsInColumn(IReadOnlyList<TileOnBoard> tiles)
    {
        var allTilesAlongReferenceTiles = tiles.ToList();
        var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
        var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
        allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

        var tilesUp = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
        var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
        allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

        var tilesBottom = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
        var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
        allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private static bool AreNumbersConsecutive(IReadOnlyCollection<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

    private void SetPlayerTurn(int playerId)
    {
        _repository.SetPlayerTurn(playerId);
        Game.Players.First(p => p.Id == playerId).SetTurn(true);
    }

    private List<TileOnBoard> GetTiles(IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        //tileId, coordinates Color Shape 
        return tilesTupleToPlay.Select(tileTupleToPlay =>
            new TileOnBoard(_repository.GetTileById(tileTupleToPlay.tileId), tileTupleToPlay.coordinates)).ToList();
    }

    private List<TileOnPlayer> GetPlayerTiles(int playerId, IEnumerable<int> tilesIds) => tilesIds.Select(tileId => _repository.GetTileOnPlayerById(playerId, tileId)).ToList();

    private static List<RackPosition> PositionsInRack(IEnumerable<TileOnPlayer> tiles) => tiles.Select(tile => tile.RackPosition).ToList();
}