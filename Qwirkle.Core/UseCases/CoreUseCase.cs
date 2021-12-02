namespace Qwirkle.Core.UseCases;

public class CoreUseCase
{
    private const int TilesNumberPerPlayer = 6;

    private readonly IRepository _repository;
    private readonly INotification _notification;
    private readonly InfoUseCase _infoUseCase;
    private readonly ComplianceUseCase _complianceUseCase;

    public Game Game { get; set; }

    public CoreUseCase(IRepository repository, INotification notification, InfoUseCase infoUseCase, ComplianceUseCase complianceUseCase)
    {
        _repository = repository;
        _notification = notification;
        _infoUseCase = infoUseCase;
        _complianceUseCase = complianceUseCase;
    }

    public List<Player> CreateGame(List<int> usersIds)
    {
        Game = _repository.CreateGame(DateTime.UtcNow);
        CreatePlayers(usersIds);
        CreateTiles();
        DealTilesToPlayers();
        SelectFirstPlayer();
        return Game.Players;
    }

    public ArrangeRackReturn TryArrangeRack(int playerId, List<int> tilesIds)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        var tiles = GetTiles(tilesIds);
        if (!player.HasTiles(tiles)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        ArrangeRack(player, tilesIds);
        return new ArrangeRackReturn { Code = PlayReturnCode.Ok };
    }

    public PlayReturn TryPlayTiles(int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        if (!player.IsTurn) return new PlayReturn(player.GameId, PlayReturnCode.NotPlayerTurn, null, null, 0);

        var tilesTuplesList = tilesTupleToPlay.ToList();
        var tilesToPlay = GetTilesOnBoard(tilesTuplesList);

        InitializeGame(player.GameId);
        if (!player.HasTiles(tilesToPlay)) return new PlayReturn(player.GameId, PlayReturnCode.PlayerDoesntHaveThisTile, null, null, 0);

        var playReturn = GetPlayReturn(tilesToPlay, player);
        if (playReturn.Code != PlayReturnCode.Ok) return playReturn;

        playReturn = playReturn with { NewRack = PlayTiles(player, tilesTuplesList, playReturn.Points) };
        _notification?.SendTilesPlayed(Game.Id, playerId, playReturn.Points, playReturn.TilesPlayed);
        _notification?.SendPlayerIdTurn(Game.Id, _infoUseCase.GetPlayerIdTurn(Game.Id));
        return playReturn;
    }

    public SwapTilesReturn TrySwapTiles(int playerId, IEnumerable<int> tilesIds)
    {
        var tilesIdsList = tilesIds.ToList();
        var player = _infoUseCase.GetPlayer(playerId);
        var tiles = GetTiles(tilesIdsList);
        InitializeGame(player.GameId);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tiles)) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.PlayerDoesntHaveThisTile };
        var swapTilesReturn = SwapTiles(player, tilesIdsList);
        _notification.SendTilesSwapped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, _infoUseCase.GetPlayerIdTurn(Game.Id));
        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        InitializeGame(player.GameId);
        var skipTurnReturn = player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (skipTurnReturn.Code != PlayReturnCode.Ok) return skipTurnReturn;
        _notification.SendTurnSkipped(Game.Id, playerId);
        _notification.SendPlayerIdTurn(Game.Id, _infoUseCase.GetPlayerIdTurn(Game.Id));
        return skipTurnReturn;
    }

    public PlayReturn TryPlayTilesSimulation(int playerId, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        var player = _infoUseCase.GetPlayer(playerId);
        var tilesToPlay = GetTilesOnBoard(tilesTupleToPlay);
        InitializeGame(player.GameId);
        return GetPlayReturn(tilesToPlay, player, true);
    }

    private PlayReturn TryPlayTilesSimulationIA(Player player, List<TileOnBoard> tilesToPlay) => GetPlayReturn(tilesToPlay, player, true);

    private void ArrangeRack(Player player, List<int> tilesIds) => _repository.ArrangeRack(player, tilesIds);

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
        if (Game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn(Game.Id, PlayReturnCode.Ok, tilesPlayed, null, 1);
        if (IsCoordinatesNotFree()) return new PlayReturn(Game.Id, PlayReturnCode.NotFree, null, null, 0);
        if (IsBoardNotEmpty() && IsAnyTileIsolated()) return new PlayReturn(Game.Id, PlayReturnCode.TileIsolated, null, null, 0);

        var wonPoints = _complianceUseCase.ComputePoints(Game, tilesPlayed);
        if (wonPoints == 0) return new PlayReturn(Game.Id, PlayReturnCode.TilesDoesntMakedValidRow, null, null, 0);

        if (IsGameFinished())
        {
            const int endGameBonusPoints = 6;
            wonPoints += endGameBonusPoints;
            if (!simulationMode) _repository.SetGameOver(Game.Id);
        }
        return new PlayReturn(Game.Id, PlayReturnCode.Ok, tilesPlayed, null, wonPoints);

        bool IsGameFinished() => IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsBagEmpty() => Game.Bag?.Tiles.Count == 0;
        bool IsBoardNotEmpty() => Game.Board.Tiles.Count > 0;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => Game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !Game.Board.IsFreeTile(tile));
    }

    public List<PlayReturn> ComputeDoableMoves(int gameId, int userId)
    {
        var player = _repository.GetPlayer(gameId, userId);
        var board = _repository.GetGame(gameId).Board;
        InitializeGame(player.GameId);
        var rack = player.Rack.GetRackWithoutDuplicates();


        var boardAdjoiningCoordinates = board.GetAdjoiningCoordinatesToTiles();

        var playReturnsWith1Tile = new List<PlayReturn>();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            foreach (var tile in rack.Tiles)
            {
                var playReturn = TryPlayTilesSimulationIA(player, new List<TileOnBoard> { TileOnBoard.From(tile, coordinates) });
                if (playReturn.Code == PlayReturnCode.Ok) playReturnsWith1Tile.Add(playReturn);
            }
        }
        playReturnsWith1Tile = playReturnsWith1Tile.OrderByDescending(p => p.Points).ToList();

        var playReturnsWith2Tiles = new List<PlayReturn>();
        foreach (var playReturn in playReturnsWith1Tile)
        {
            var tilePlayed = playReturn.TilesPlayed[0];
            var currentTilesToTest = rack.Tiles.Where(t => t != tilePlayed).ToList();

            var firstGameMove = board.Tiles.Count == 0;
            if (firstGameMove)
            {
                playReturnsWith2Tiles.AddRange(ComputePlayReturnWith2TilesInRow(RandomRowType(), player, boardAdjoiningCoordinates, currentTilesToTest, tilePlayed, true));
            }
            else
            {
                foreach (RowType rowType in Enum.GetValues(typeof(RowType)))
                    playReturnsWith2Tiles.AddRange(ComputePlayReturnWith2TilesInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, tilePlayed, false));
            }
        }
        playReturnsWith2Tiles = playReturnsWith2Tiles.OrderBy(p => p.Points).ToList();


        var playReturnsWith3Tiles = new List<PlayReturn>();
        foreach (var playReturn in playReturnsWith2Tiles)
        {
            var firstTilePlayed = playReturn.TilesPlayed[0];
            var secondTilePlayed = playReturn.TilesPlayed[1];
            var rowType = firstTilePlayed.Coordinates.X == secondTilePlayed.Coordinates.X ? RowType.Column : RowType.Line;

            var currentTilesToTest = rack.Tiles.Where(t => t != firstTilePlayed && t != secondTilePlayed).ToList();
            playReturnsWith3Tiles.AddRange(ComputePlayReturnWith3TilesInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, firstTilePlayed, secondTilePlayed));
        }

        //we have all possible moves with 3 tiles :)
        var allPlayReturns = new List<PlayReturn>();
        allPlayReturns.AddRange(playReturnsWith3Tiles);
        allPlayReturns.AddRange(playReturnsWith2Tiles);
        allPlayReturns.AddRange(playReturnsWith1Tile);

        return allPlayReturns;


        static RowType RandomRowType()
        {
            var rowTypeValues = typeof(RowType).GetEnumValues();
            var index = new Random().Next(rowTypeValues.Length);
            return (RowType)rowTypeValues.GetValue(index)!;
        }
    }

    private IEnumerable<PlayReturn> ComputePlayReturnWith3TilesInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> rackTiles, TileOnBoard firstTile, TileOnBoard secondTile)
    {
        var (firstTilePlayedX, firstTilePlayedY) = firstTile.Coordinates;
        var (secondTilePlayedX, secondTilePlayedY) = secondTile.Coordinates;

        var coordinateChangingMax = rowType is RowType.Line ? Math.Max(firstTilePlayedX, secondTilePlayedX) : Math.Max(firstTilePlayedY, secondTilePlayedY);
        var coordinateChangingMin = rowType is RowType.Line ? Math.Min(firstTilePlayedX, secondTilePlayedX) : Math.Min(firstTilePlayedY, secondTilePlayedY);

        var coordinateFixed = rowType is RowType.Line ? firstTilePlayedY : firstTilePlayedX;
        var playReturnsWith3Tiles = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
            : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (coordinateChangingMax == boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + 1);
        if (coordinateChangingMin == boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMin - 1);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMax);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMin);

        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in rackTiles)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var playReturn2 = TryPlayTilesSimulationIA(player, new List<TileOnBoard> { firstTile, secondTile, testedTile });
                if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith3Tiles.Add(playReturn2);
            }
        }
        return playReturnsWith3Tiles;
    }

    private IEnumerable<PlayReturn> ComputePlayReturnWith2TilesInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> tilesToTest, TileOnBoard firstTile, bool firstGameMove)
    {
        var (tilePlayedX, tilePlayedY) = firstTile.Coordinates;
        var coordinateChanging = rowType is RowType.Line ? tilePlayedX : tilePlayedY;
        var coordinateFixed = rowType is RowType.Line ? tilePlayedY : tilePlayedX;
        var playReturnsWith2Tiles = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
                                            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
                                          : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (!firstGameMove)
        {
            if (coordinateChanging == boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChanging + 1);
            if (coordinateChanging == boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChanging - 1);
        }
        else
        {
            var addOrSubtract1Unit = new Random().Next(2) * 2 - 1;
            boardAdjoiningCoordinatesRow.Add(coordinateChanging + addOrSubtract1Unit);
        }

        boardAdjoiningCoordinatesRow.Remove(coordinateChanging);
        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in tilesToTest)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var playReturn2 = TryPlayTilesSimulationIA(player, new List<TileOnBoard> { firstTile, testedTile });
                if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith2Tiles.Add(playReturn2);
            }
        }
        return playReturnsWith2Tiles;
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

    private SwapTilesReturn SwapTiles(Player player, IEnumerable<int> tilesIds)
    {
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        var tilesIdsList = tilesIds.ToList();
        for (byte i = 0; i < tilesIdsList.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tilesIdsList);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = _infoUseCase.GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Player player, IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay, int points)
    {
        var tilesTupleToPlayList = tilesTupleToPlay.ToList();
        var tilesToPlay = GetTilesOnBoard(tilesTupleToPlayList);

        player.LastTurnPoints = points;
        player.Points += points;
        Game.Board.Tiles.AddRange(tilesToPlay);
        SetNextPlayerTurnToPlay(player);
        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlay.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(Game.Id, player.Id, tilesTupleToPlayList);
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

    private IEnumerable<Tile> GetTiles(IEnumerable<int> tilesIds) => tilesIds.Select(id => _repository.GetTile(id));

    private List<TileOnBoard> GetTilesOnBoard(IEnumerable<(int tileId, Coordinates coordinates)> tilesTupleToPlay)
    {
        return tilesTupleToPlay.Select(tileTupleToPlay => new TileOnBoard(_repository.GetTile(tileTupleToPlay.tileId), tileTupleToPlay.coordinates)).ToList();
    }
}