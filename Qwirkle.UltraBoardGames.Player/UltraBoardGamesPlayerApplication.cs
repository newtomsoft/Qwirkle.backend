namespace Qwirkle.UltraBoardGames.Player;

public class UltraBoardGamesPlayerApplication
{
    private const int TilesNumberPerPlayer = 6;
    private readonly ILogger _logger;
    private readonly BotUseCase _botUseCase;
    private readonly GameScraper _scraper;
    private readonly Coordinates _originCoordinates = Coordinates.From(25, 25);

    public UltraBoardGamesPlayerApplication(ILogger<UltraBoardGamesPlayerApplication> logger, BotUseCase botUseCase, GameScraper gameScraper)
    {
        _logger = logger;
        _botUseCase = botUseCase;
        _scraper = gameScraper;
    }

    public void Run()
    {
        LogStartApplication();
        for (var i = 0; i < 3; i++) PlayGame();
        LogEndApplication();
    }

    private void PlayGame()
    {
        LogStartGame();
        _scraper.GoToGame();
        _scraper.AcceptPolicies();
        var board = Board.Empty();
        GameStatus gameStatus;
        var lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
        while (true)
        {
            HashSet<TileOnBoard> tilesPlayedByOpponent;
            var timeOut = DateTime.Now.AddSeconds(2);
            do
            {
                tilesPlayedByOpponent = _scraper.GetTilesPlayedByOpponent();
                Task.Delay(50);
            } while ((tilesPlayedByOpponent.Count == 0 || lastTilesPlayedByOpponent.SetEquals(tilesPlayedByOpponent)) && timeOut > DateTime.Now);
            lastTilesPlayedByOpponent = new HashSet<TileOnBoard>();
            lastTilesPlayedByOpponent.UnionWith(tilesPlayedByOpponent);
            board.AddTiles(tilesPlayedByOpponent);
            _scraper.TakeScreenShot();
            gameStatus = _scraper.GetGameStatus();
            if (gameStatus != GameStatus.InProgress) break;
            var playerPoints = _scraper.GetPlayerPoints();
            var opponentPoints = _scraper.GetOpponentPoints();
            var tilesOnPlayer = _scraper.GetTilesOnPlayer();
            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };

            var game = new Game(board, players);
            var tilesToPlay = board.Tiles.Count > 0
                ? _botUseCase.GetMostPointsMove(bot, game).ToList()
                : _botUseCase.GetMostPointsMove(bot, game, _originCoordinates).ToList();

            if (tilesToPlay.Count == 0)
            {
                var tilesOnBagNumber = _scraper.GetTilesOnBag();
                _scraper.Swap(Math.Min(tilesOnBagNumber, TilesNumberPerPlayer));
                _scraper.TakeScreenShot();
                continue;
            }

            List<TileOnBoard> otherTilesToPlay;
            var tilesToPlayStillHere = new List<TileOnBoard>();
            var tilesToPlayArranged = new List<TileOnBoard>();
            tilesToPlayStillHere.AddRange(tilesToPlay);
            var boardCopy = Board.From(board);
            do
            {
                var firstTilesToPlay = boardCopy.Tiles.Count == 0
                    ? tilesToPlayStillHere.Where(tile => tile.Coordinates == _originCoordinates).ToList()
                    : tilesToPlayStillHere.Where(tile => boardCopy.IsIsolatedTile(tile)).ToList();
                otherTilesToPlay = tilesToPlayStillHere.Except(firstTilesToPlay).ToList();
                tilesToPlayArranged.AddRange(firstTilesToPlay);
                if (otherTilesToPlay.Count == 0) break;
                boardCopy.AddTiles(firstTilesToPlay);
                tilesToPlayStillHere = otherTilesToPlay;
            } while (otherTilesToPlay.Count != 0);

            _scraper.Play(tilesToPlayArranged);
            board.AddTiles(tilesToPlay);
            _scraper.TakeScreenShot();
        }
        _scraper.CloseEndWindow();
        LogEndGame(gameStatus);
    }

    private void LogStartApplication() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
    private void LogStartGame() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
    private void LogEndGame(GameStatus gameStatus) => _logger.LogInformation("{applicationEvent} at {dateTime}", $"{gameStatus}", DateTime.UtcNow);
    private void LogEndApplication() => _logger.LogInformation("{applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);
    private static Domain.Entities.Player Player(int playerPoints, List<TileOnPlayer> tilesOnPlayer, bool isTurn) => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}