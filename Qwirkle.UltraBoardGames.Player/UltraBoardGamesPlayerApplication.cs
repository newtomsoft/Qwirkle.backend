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
        LogStartApplication();
        for (var i = 0; i < 10; i++)
        {
            PlayGame();
        }
        LogEndApplication();
        _scraper.Dispose();
    }

    private void PlayGame()
    {
        LogStartGame();
        _scraper.GoToGame();
        _scraper.AcceptPolicies();
        var board = Board.Empty();
        GameStatus gameStatus;
        var lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
        int playerPoints = 0, opponentPoints = 0;
        while (true)
        {
            var tilesPlayedByOpponent = TilesPlayedByOpponent(lastTilesPlayedByOpponent);
            lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
            lastTilesPlayedByOpponent.UnionWith(tilesPlayedByOpponent);
            board.AddTiles(tilesPlayedByOpponent);
            gameStatus = _scraper.GetGameStatus();
            if (gameStatus != GameStatus.InProgress) break;
            playerPoints = _scraper.GetPlayerPoints();
            opponentPoints = _scraper.GetOpponentPoints();
            var tilesOnPlayer = _scraper.GetTilesOnPlayer();
            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };
            var game = new Game(board, players);
            var tilesToPlay = board.Tiles.Count > 0
                ? _botService.GetMostPointsTilesToPlay(bot, game).Tiles
                : _botService.GetMostPointsTilesToPlay(bot, game, _originCoordinates).Tiles;

            _scraper.TakeScreenShot();

            if (tilesToPlay.Count == 0)
            {
                SwapOrSkipTurn();
                continue;
            }
            var tilesToPlayOrdered = TilesToPlayOrdered(tilesToPlay, board);
            _scraper.Play(tilesToPlayOrdered);
            board.AddTiles(tilesToPlay);
            _scraper.TakeScreenShot();
        }
        _scraper.CloseEndWindow();
        var gameInformation = new GameInformation(gameStatus, playerPoints, opponentPoints);
        LogEndGame(gameInformation);
    }

    private List<TileOnBoard> TilesToPlayOrdered(List<TileOnBoard> tilesToPlay, Board board)
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

    private void SwapOrSkipTurn()
    {
        var tilesOnBagNumber = _scraper.GetTilesOnBag();
        var tilesToSwapNumber = Math.Min(tilesOnBagNumber, CoreService.TilesNumberPerPlayer);
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
        foreach (var (tile, coordinates) in tilesPlayedByOpponent)
        {
            _logger?.LogInformation("{applicationEvent} {tile} to {coordinates} at {dateTime}", "Opponent move tile", tile, coordinates, DateTime.Now);
        }
    }

    private void LogStartApplication() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Started", DateTime.Now);
    private void LogStartGame() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Started", DateTime.Now);
    private void LogEndGame(GameInformation gameInformation) => _logger?.LogInformation("{wonOrLost} by {playerPoints} vs {opponentPoints} at {dateTime}", gameInformation.Status, gameInformation.PlayerPoints, gameInformation.OpponentPoints, DateTime.Now);
    private void LogEndApplication() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Ended", DateTime.Now);
    private static Domain.Entities.Player Player(int playerPoints, List<TileOnPlayer> tilesOnPlayer, bool isTurn) => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}