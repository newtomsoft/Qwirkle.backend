namespace Qwirkle.UltraBoardGames.Player;

public class UltraBoardGamesPlayerApplication
{
    private readonly ILogger _logger;
    private readonly BotService _botService;
    private readonly GameScraper _scraper;
    private readonly Coordinates _originCoordinates = Coordinates.From(25, 25);

    public UltraBoardGamesPlayerApplication(ILogger<UltraBoardGamesPlayerApplication> logger, BotService botService, GameScraper gameScraper)
    {
        _logger = logger;
        _botService = botService;
        _scraper = gameScraper;
    }

    public void Run()
    {
        _logger.LogInformation("Application started");
        for (var i = 0; i < 10; i++)
        {
            PlayGame();
        }
        _logger.LogInformation("Ended");
    }

    private void PlayGame()
    {
        _logger.LogInformation("Game started");
        _scraper.GoToGame();
        _scraper.AcceptPolicies();
        _scraper.CleanWindow();
        var board = Board.Empty();
        GameStatus gameStatus;
        var lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
        int playerPoints, opponentPoints;
        while (true)
        {
            _scraper.AdjustBoardView();
            var tilesPlayedByOpponent = TilesPlayedByOpponent(lastTilesPlayedByOpponent);
            lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
            lastTilesPlayedByOpponent.UnionWith(tilesPlayedByOpponent);
            board.AddTiles(tilesPlayedByOpponent);
            gameStatus = _scraper.GetGameStatus();
            playerPoints = _scraper.GetPlayerPoints();
            opponentPoints = _scraper.GetOpponentPoints();

            if (gameStatus != GameStatus.InProgress) break;

            var tilesOnPlayer = _scraper.GetTilesOnPlayer();
            var tilesNumberOnBag = _scraper.GetTilesOnBag();
            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };
            var game = new Game(board, players, tilesNumberOnBag);
            var tilesToPlay = board.Tiles.Count > 0 ? _botService.GetBestMove(bot, game).Tiles : _botService.GetBestMove(bot, game, _originCoordinates).Tiles;

            _scraper.TakeScreenShot();
            if (tilesToPlay.Count == 0)
            {
                SwapOrSkipTurn(tilesNumberOnBag);
                continue;
            }
            var tilesToPlayOrdered = TilesToPlayOrdered(tilesToPlay, board);
            _scraper.Play(tilesToPlayOrdered);
            board.AddTiles(tilesToPlay);
            _scraper.TakeScreenShot();
        }
        _scraper.CloseEndWindow();
        _logger.LogInformation("{wonOrLost} by {playerPoints} vs {opponentPoints}", gameStatus, playerPoints, opponentPoints);
    }

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
                ? tilesToPlayStillHere.Where(tile => tile.Coordinates == _originCoordinates).ToList()
                : tilesToPlayStillHere.Where(tile => boardCopy.IsIsolatedTile(tile)).ToList();
            otherTilesToPlay = tilesToPlayStillHere.Except(firstTilesToPlay).ToList();
            tilesToPlayOrdered.AddRange(firstTilesToPlay);
            if (otherTilesToPlay.Count == 0) break;
            boardCopy.AddTiles(firstTilesToPlay);
            tilesToPlayStillHere = otherTilesToPlay;
        } while (otherTilesToPlay.Count != 0);

        return tilesToPlayOrdered;
    }

    private void SwapOrSkipTurn(int tilesNumberOnBag)
    {
        var tilesToSwapNumber = Math.Min(tilesNumberOnBag, CoreService.TilesNumberPerPlayer);
        if (tilesToSwapNumber > 0) _scraper.Swap(tilesToSwapNumber);
        else _scraper.Skip();
        _scraper.TakeScreenShot();
    }

    private HashSet<TileOnBoard> TilesPlayedByOpponent(IReadOnlySet<TileOnBoard> lastTilesPlayedByOpponent)
    {
        HashSet<TileOnBoard> tilesPlayedByOpponent;
        var timeOut = DateTime.Now.AddSeconds(2);
        do
        {
            tilesPlayedByOpponent = _scraper.GetTilesPlayedByOpponent();
            Task.Delay(50);
        } while ((tilesPlayedByOpponent.Count == 0 || lastTilesPlayedByOpponent.SetEquals(tilesPlayedByOpponent)) && timeOut > DateTime.Now);

        LogTilesPlayedByOpponent(tilesPlayedByOpponent);
        return tilesPlayedByOpponent;
    }

    private void LogTilesPlayedByOpponent(HashSet<TileOnBoard> tilesPlayedByOpponent)
    {
        foreach (var tile in tilesPlayedByOpponent) _logger?.LogInformation("Opponent move {tile}", tile);
    }
    private static Domain.Entities.Player Player(int playerPoints, List<TileOnPlayer> tilesOnPlayer, bool isTurn) => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}