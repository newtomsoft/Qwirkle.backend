namespace Qwirkle.Domain.UseCases;

public class BotUseCase
{
    public readonly InfoUseCase _infoUseCase;
    private readonly CoreUseCase _coreUseCase;
    private readonly ILogger<CoreUseCase> _logger;
    private Game _game;
    private const int TilesNumberPerPlayer = 6;

    public BotUseCase(InfoUseCase infoUseCase, CoreUseCase coreUseCase, ILogger<CoreUseCase> logger)
    {
        _infoUseCase = infoUseCase;
        _coreUseCase = coreUseCase;
        _logger = logger;
    }

    public void Play(Game game, Player bot)
    {
        
        var tilesToPlay = GetMostPointsTilesToPlay(bot, game).ToList();

        if (tilesToPlay.Count > 0 && game.GameOver!=true) _coreUseCase.TryPlayTiles(bot.Id, tilesToPlay);
        else SwapOrSkipTurn(bot, game.Bag.Tiles.Count);

        if (tilesToPlay.Count > 0)
        {
            _logger?.LogInformation($"Bot play {tilesToPlay.ToLog()}");
            _coreUseCase.TryPlayTiles(bot.Id, tilesToPlay);
        }
        else
        {
            _logger?.LogInformation("Bot swap or skip...");
            SwapOrSkipTurn(bot, game.Bag.Tiles.Count);
        }

    }
    public int GetMostPointsToPlay(Player player, Game game, Coordinates originCoordinates = null)
    {
        _game = game;
        var doableMoves = ComputeDoableMoves(player, originCoordinates, true);
        var playReturn = doableMoves.OrderByDescending(m => m.Points).FirstOrDefault();
        return playReturn?.Points ?? 0;
    }

    public IEnumerable<TileOnBoard> GetMostPointsTilesToPlay(Player player, Game game, Coordinates originCoordinates = null)
    {
        _game = game;
        var doableMoves = ComputeDoableMoves(player, originCoordinates, true);
        var playReturn = doableMoves.OrderByDescending(m => m.Points).FirstOrDefault();
        return playReturn?.TilesPlayed ?? new List<TileOnBoard>();
    }

    public List<PlayReturn> ComputeDoableMoves(int gameId, int userId)
    {
        var player = _infoUseCase.GetPlayer(gameId, userId);
        _game = _infoUseCase.GetGame(gameId);
        return ComputeDoableMoves(player);
    }



    private List<PlayReturn> ComputeDoableMoves(Player player, Coordinates originCoordinates = null, bool simulation = false)
    {
        if (!simulation) _coreUseCase.ResetGame(player.GameId);
        var rack = player.Rack.WithoutDuplicatesTiles();

        var boardAdjoiningCoordinates = _game.Board.GetFreeAdjoiningCoordinatesToTiles(originCoordinates);

        var allPlayReturns = new List<PlayReturn>();
        var playReturnsWith1Tile = new List<PlayReturn>();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            foreach (var tile in rack.Tiles)
            {
                var playReturn = TestPlayTiles(player, new List<TileOnBoard> { TileOnBoard.From(tile, coordinates) });
                Console.WriteLine(playReturn.Code);
                if (playReturn.Code == PlayReturnCode.Ok) playReturnsWith1Tile.Add(playReturn);
            }
        }

        allPlayReturns.AddRange(playReturnsWith1Tile);
        var lastPlayReturn = playReturnsWith1Tile;
        for (var tilePlayedNumber = 2; tilePlayedNumber <= TilesNumberPerPlayer; tilePlayedNumber++)
        {
            var currentPlayReturns = new List<PlayReturn>();
            foreach (var playReturn in lastPlayReturn)
            {
                var tilesPlayed = playReturn.TilesPlayed;
                var currentTilesToTest = rack.Tiles.Select(t => t.ToTile()).Except(tilesPlayed.Select(tP => tP.ToTile())).Select((t, index) => t.ToTileOnPlayer((RackPosition)index)).ToList();
                if (_game.IsBoardEmpty() && tilePlayedNumber == 2) // todo ok but can do better
                {
                    currentPlayReturns.AddRange(ComputePlayReturnInRow(RandomRowType(), player, boardAdjoiningCoordinates, currentTilesToTest, tilesPlayed, true));
                }
                else
                {
                    foreach (RowType rowType in Enum.GetValues(typeof(RowType)))
                        currentPlayReturns.AddRange(ComputePlayReturnInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, tilesPlayed, false));
                }
            }
            allPlayReturns.AddRange(currentPlayReturns);
            lastPlayReturn = currentPlayReturns;
        }
        return allPlayReturns;
    }

    private IEnumerable<PlayReturn> ComputePlayReturnInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> tilesToTest, List<TileOnBoard> tilesAlreadyPlayed, bool firstGameMove)
    {
        int tilesPlayedNumber = tilesAlreadyPlayed.Count;
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

        var playReturns = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
                                            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
                                          : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (!firstGameMove)
        {
            if (coordinateChangingMax >= boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + 1);
            if (coordinateChangingMin <= boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMin - 1);
        }
        else
        {
            var addOrSubtract1Unit = Random.Shared.Next(2) * 2 - 1;
            boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + addOrSubtract1Unit);
            // we have coordinateChangingMax = coordinateChangingMin
        }
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMax);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMin);

        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in tilesToTest)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var currentTilesToTest = new List<TileOnBoard>();
                currentTilesToTest.AddRange(tilesAlreadyPlayed);
                currentTilesToTest.Add(testedTile);
                var playReturn = TestPlayTiles(player, currentTilesToTest);
                if (playReturn.Code == PlayReturnCode.Ok) playReturns.Add(playReturn);
            }
        }
        return playReturns;
    }

    private PlayReturn TestPlayTiles(Player player, List<TileOnBoard> tilesToPlay) => _coreUseCase.Play(tilesToPlay, player, _game, true);

    private static RowType RandomRowType()
    {
        var rowTypeValues = typeof(RowType).GetEnumValues();
        var index = new Random().Next(rowTypeValues.Length);
        return (RowType)rowTypeValues.GetValue(index)!;
    }

    private void SwapOrSkipTurn(Player bot, int tilesOnBagNumber)
    {
        var tilesToSwapMaxNumber = Math.Min(tilesOnBagNumber, TilesNumberPerPlayer);
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

    private void Swap(Player bot, int tilesToSwapNumber)
    {
        _coreUseCase.TrySwapTiles(bot.Id, bot.Rack.Tiles.Take(tilesToSwapNumber));
        //todo make algorithm to select tiles to swap 
    }

    private void Skip(int botId) => _coreUseCase.TrySkipTurn(botId);
}