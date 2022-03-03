namespace Qwirkle.Domain.Services;

public class BotService
{
    private readonly InfoService _infoService;
    private readonly CoreService _coreService;
    private readonly ILogger<CoreService> _logger;

    public BotService(InfoService infoService, CoreService coreService, ILogger<CoreService> logger)
    {
        _infoService = infoService;
        _coreService = coreService;
        _logger = logger;
    }

    public void Play(Game game, Player bot)
    {
        var tilesToPlay = GetBestMove(bot, game).Tiles.ToList();
        if (tilesToPlay.Count > 0)
        {
            _logger?.LogInformation($"Bot play {tilesToPlay.ToLog()}");
            _coreService.TryPlayTiles(bot.Id, tilesToPlay);
        }
        else
        {
            _logger?.LogInformation("Bot swap or skip...");
            SwapOrSkipTurn(bot, game.Bag.Tiles.Count);
        }
    }

    public int GetMostPointsToPlay(Player player, Game game, Coordinates originCoordinates = null)
    {
        var doableMoves = ComputeDoableMoves(player, game, originCoordinates, true);
        var playReturn = doableMoves.OrderByDescending(m => m.Move.Points).FirstOrDefault();
        return playReturn?.Move.Points ?? 0;
    }

    public Move GetBestMove(Player player, Game game, Coordinates originCoordinates = null)
    {
        var moves = ComputeDoableMoves(player, game, originCoordinates, true).Select(r => r.Move).OrderByDescending(m => m.Points).ToList();

        if (moves.Count == 0) return Move.Empty;

        var moveWithMaxPoints = moves.First();
        var moveWithMaxTilesAndPoint = moves.OrderByDescending(m => m.TilesNumber).ThenByDescending(m => m.Points).First();

        if (player.Rack.TilesNumber == moveWithMaxTilesAndPoint.TilesNumber) return moveWithMaxTilesAndPoint;
        if (moveWithMaxPoints!.Points >= CoreService.PointsForAQwirkle) return moveWithMaxPoints;

        var tilesInBagNumber = game.Bag.TilesNumber;
        if (tilesInBagNumber == 0 || moveWithMaxTilesAndPoint!.TilesNumber > tilesInBagNumber + 1) return moveWithMaxTilesAndPoint;

        var moveUsingDuplicateTile = MoveUsingDuplicateTile(moves, player.Rack);
        if (moveUsingDuplicateTile.TilesNumber > 0 && moveUsingDuplicateTile.Points >= moveWithMaxPoints.Points * 2 / 3) return moveUsingDuplicateTile;

        return moveWithMaxPoints;
    }

    public HashSet<PlayReturn> ComputeDoableMoves(int gameId, int userId)
    {
        var player = _infoService.GetPlayer(gameId, userId);
        var game = _infoService.GetGame(gameId);
        return ComputeDoableMoves(player, game);
    }

    private HashSet<PlayReturn> ComputeDoableMoves(Player player, Game game, Coordinates originCoordinates = null, bool simulation = false)
    {
        if (!simulation) _coreService.ResetGame(player.GameId);

        var rack = player.Rack.WithoutDuplicatesTiles();
        var boardAdjoiningCoordinates = game.Board.GetFreeAdjoiningCoordinatesToTiles(originCoordinates);
        var with1TilePlayReturns = new HashSet<PlayReturn>();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            foreach (var tile in rack.Tiles)
            {
                var playReturn = TestPlayTiles(player, new HashSet<TileOnBoard> { TileOnBoard.From(tile, coordinates) }, game);
                if (playReturn.Code == ReturnCode.Ok) with1TilePlayReturns.Add(playReturn);
            }
        }
        var allPlayReturns = new HashSet<PlayReturn>(with1TilePlayReturns);

        var rowTypes = game.IsBoardEmpty() ? new List<RowType> { RandomRowType() } : ((RowType[])Enum.GetValues(typeof(RowType))).ToList();
        foreach (var rowType in rowTypes)
        {
            var lastPlayReturn = with1TilePlayReturns;
            for (var tilePlayedNumber = 2; tilePlayedNumber <= CoreService.TilesNumberPerPlayer; tilePlayedNumber++)
            {
                var currentPlayReturns = new HashSet<PlayReturn>();
                foreach (var tilesPlayed in lastPlayReturn.Select(p => p.Move.Tiles))
                {
                    var currentTilesToTest = rack.Tiles.Select(t => t.ToTile()).Except(tilesPlayed.Select(tP => tP.ToTile())).Select((t, index) => t.ToTileOnPlayer((RackPosition)index)).ToList();
                    currentPlayReturns.UnionWith(ComputePlayReturnInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, tilesPlayed.ToHashSet(), game));
                }
                allPlayReturns.UnionWith(currentPlayReturns);
                lastPlayReturn = currentPlayReturns;
            }
        }
        return allPlayReturns;
    }

    private IEnumerable<PlayReturn> ComputePlayReturnInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> tilesToTest, HashSet<TileOnBoard> tilesAlreadyPlayed, Game game)
    {
        var tilesPlayedNumber = tilesAlreadyPlayed.Count;
        var coordinatesPlayed = tilesAlreadyPlayed.Select(tilePlayed => tilePlayed.Coordinates).ToList();

        sbyte coordinateChangingMin, coordinateChangingMax;
        var firstTilePlayedX = coordinatesPlayed[0].X;
        var firstTilePlayedY = coordinatesPlayed[0].Y;
        if (tilesPlayedNumber >= 2)
        {
            coordinateChangingMax = rowType is RowType.Line ? coordinatesPlayed.Max(c => c.X) : coordinatesPlayed.Max(c => c.Y);
            coordinateChangingMin = rowType is RowType.Line ? coordinatesPlayed.Min(c => c.X) : coordinatesPlayed.Min(c => c.Y);
        }
        else
        {
            coordinateChangingMax = rowType is RowType.Line ? firstTilePlayedX : firstTilePlayedY;
            coordinateChangingMin = coordinateChangingMax;
        }

        var coordinateFixed = rowType is RowType.Line ? coordinatesPlayed.First().Y : coordinatesPlayed.First().X;

        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
            : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (game.IsBoardEmpty() && tilesAlreadyPlayed.Count == 1)
        {
            var addOrSubtract1Unit = Random.Shared.Next(2) * 2 - 1;
            boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + addOrSubtract1Unit);
            // we have coordinateChangingMax = coordinateChangingMin
        }
        else
        {
            if (coordinateChangingMax >= boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + 1);
            if (coordinateChangingMin <= boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMin - 1);
        }

        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMax);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMin);

        var playReturns = new HashSet<PlayReturn>();
        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in tilesToTest)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var currentTilesToTest = new HashSet<TileOnBoard>();
                currentTilesToTest.UnionWith(tilesAlreadyPlayed);
                currentTilesToTest.Add(testedTile);
                var playReturn = TestPlayTiles(player, currentTilesToTest, game);
                if (playReturn.Code == ReturnCode.Ok) playReturns.Add(playReturn);
            }
        }
        return playReturns;
    }

    private PlayReturn TestPlayTiles(Player player, HashSet<TileOnBoard> tilesToPlay, Game game) => _coreService.Play(tilesToPlay, player, game, true);

    private static RowType RandomRowType()
    {
        var rowTypeValues = typeof(RowType).GetEnumValues();
        var index = new Random().Next(rowTypeValues.Length);
        return (RowType)rowTypeValues.GetValue(index)!;
    }

    private void SwapOrSkipTurn(Player bot, int tilesOnBagNumber)
    {
        var tilesToSwapMaxNumber = Math.Min(tilesOnBagNumber, CoreService.TilesNumberPerPlayer);
        if (tilesToSwapMaxNumber > 0)
        {
            _logger?.LogInformation($"Bot swap {tilesToSwapMaxNumber} tiles");
            Swap(bot, tilesToSwapMaxNumber);
        }
        else
        {
            _logger?.LogInformation("Bot skip turn");
            Skip(bot.Id);
        }
    }

    private static Move MoveUsingDuplicateTile(IEnumerable<Move> moves, Rack rack)
    {
        foreach (var move in moves)
        {
            foreach (var tile in move.Tiles)
            {
                var tilesInRack = rack.Tiles.Select(t => t.ToTile()).ToList();
                tilesInRack.Remove(tile.ToTile());
                if (tilesInRack.Contains(tile.ToTile())) return move;
            }
        }
        return Move.Empty;
    }

    private void Swap(Player bot, int tilesToSwapNumber)
    {
        _coreService.TrySwapTiles(bot.Id, bot.Rack.Tiles.Take(tilesToSwapNumber));
        //todo make algorithm to select tiles to swap 
    }

    private void Skip(int botId) => _coreService.TrySkipTurn(botId);
}