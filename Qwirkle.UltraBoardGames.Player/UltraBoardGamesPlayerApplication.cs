namespace Qwirkle.UltraBoardGames.Player;

public class UltraBoardGamesPlayerApplication
{
    private readonly ILogger<UltraBoardGamesPlayerApplication> _logger;
    private readonly BotService _botService;
    private readonly IWebDriverFactory _webDriverFactory;
    private readonly Coordinate _originCoordinate = Coordinate.From(25, 25);
    private readonly List<GameScraper> _parallelScrapers = new();
    private const int ParallelScrapersNumber = 6;

    public UltraBoardGamesPlayerApplication(ILogger<UltraBoardGamesPlayerApplication> logger, BotService botService, IWebDriverFactory webDriverFactory)
    {
        _logger = logger;
        _botService = botService;
        _webDriverFactory = webDriverFactory;
    }

    public void Run(int gamesNumber)
    {
        _logger.LogInformation("Application started");
        for (var i = 0; i < gamesNumber; i++)
            _parallelScrapers.Add(new GameScraper(_logger, _webDriverFactory));

        Parallel.ForEach(_parallelScrapers, scraper =>
        {
            scraper.BeginScraping();
            PlayGame(scraper);
            scraper.CloseBrowser(TimeSpan.FromMilliseconds(800));
        });
        _logger.LogInformation("Ended");
    }

    private void PlayGame(GameScraper gameScraper)
    {
        gameScraper.GoToGame();
        gameScraper.AcceptPolicies();
        gameScraper.CleanWindow();
        var board = Board.Empty;
        GameStatus gameStatus;
        var lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
        int playerPoints = 0, opponentPoints = 0;
        Task.Delay(1000).Wait();
        while (true)
        {
            gameStatus = gameScraper.GetGameStatus();
            if (gameStatus != GameStatus.InProgress) break;
            gameScraper.AdjustBoardView();
            var tilesPlayedByOpponent = TilesPlayedByOpponent(gameScraper, lastTilesPlayedByOpponent);
            lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
            lastTilesPlayedByOpponent.UnionWith(tilesPlayedByOpponent);
            board.AddTiles(tilesPlayedByOpponent);
            playerPoints = gameScraper.GetPlayerPoints();
            opponentPoints = gameScraper.GetOpponentPoints();
            var tilesOnPlayer = gameScraper.GetTilesOnPlayer();
            var tilesNumberOnBag = gameScraper.GetTilesOnBag();
            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };
            var game = new Game(board, players, tilesNumberOnBag);
            var move = board.Tiles.Count > 0 ? _botService.GetBestMove(bot, game) : _botService.GetBestMove(bot, game, _originCoordinate);

            gameScraper.TakeScreenShot();
            if (move.TilesNumber == 0)
            {
                SwapOrSkipTurn(gameScraper, tilesNumberOnBag);
                continue;
            }
            var tilesToPlayOrdered = TilesToPlayOrdered(move.Tiles, board);
            gameScraper.Play(tilesToPlayOrdered);

            if (IsPlayingError(gameScraper))
            {
                board = Board.From(gameScraper.GetTilesOnBoard());
                continue;
            }
            board.AddTiles(move.Tiles);
            if (move.Points != gameScraper.GetPlayerLastPoints())
                board = Board.From(gameScraper.GetTilesOnBoard());

            gameScraper.TakeScreenShot();
        }
        _logger.LogInformation("{wonOrLost} by {playerPoints} vs {opponentPoints}", gameStatus.ToString(), playerPoints, opponentPoints);
        gameScraper.CloseEndGameNotificationWindow();
    }

    private bool IsPlayingError(GameScraper gameScraper) => gameScraper.IsPlayingError();

    private List<TileOnBoard> TilesToPlayOrdered(IEnumerable<TileOnBoard> tilesToPlay, Board board)
    {
        List<TileOnBoard> otherTilesToPlay;
        var tilesToPlayStillHere = new List<TileOnBoard>();
        var tilesToPlayOrdered = new List<TileOnBoard>();
        tilesToPlayStillHere.AddRange(tilesToPlay);
        var boardCopy = Board.From(board);
        do
        {
            var firstTilesToPlay = boardCopy.Tiles.Count == 0
                ? tilesToPlayStillHere.Where(tile => tile.Coordinate == _originCoordinate).ToList()
                : tilesToPlayStillHere.Where(tile => boardCopy.IsIsolatedTile(tile)).ToList();
            otherTilesToPlay = tilesToPlayStillHere.Except(firstTilesToPlay).ToList();
            tilesToPlayOrdered.AddRange(firstTilesToPlay);
            if (otherTilesToPlay.Count == 0) break;
            boardCopy.AddTiles(firstTilesToPlay);
            tilesToPlayStillHere = otherTilesToPlay;
        } while (otherTilesToPlay.Count != 0);

        return tilesToPlayOrdered;
    }

    private static void SwapOrSkipTurn(GameScraper gameScraper, int tilesNumberOnBag)
    {
        var tilesToSwapNumber = Math.Min(tilesNumberOnBag, CoreService.TilesNumberPerPlayer);
        if (tilesToSwapNumber > 0) gameScraper.Swap(tilesToSwapNumber);
        else gameScraper.Skip();
        gameScraper.TakeScreenShot();
    }

    private HashSet<TileOnBoard> TilesPlayedByOpponent(GameScraper gameScraper, IReadOnlySet<TileOnBoard> lastTilesPlayedByOpponent)
    {
        HashSet<TileOnBoard> tilesPlayedByOpponent;
        var timeOut = DateTime.Now.AddSeconds(2);
        do
        {
            tilesPlayedByOpponent = gameScraper.GetTilesPlayedByOpponent();
            Task.Delay(50);
        } while ((tilesPlayedByOpponent.Count == 0 || lastTilesPlayedByOpponent.SetEquals(tilesPlayedByOpponent)) && timeOut > DateTime.Now);

        foreach (var tile in tilesPlayedByOpponent) _logger?.LogInformation("Opponent move {tile}", tile);
        return tilesPlayedByOpponent;
    }

    private static Domain.Entities.Player Player(int playerPoints, List<TileOnRack> tilesOnPlayer, bool isTurn) => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}