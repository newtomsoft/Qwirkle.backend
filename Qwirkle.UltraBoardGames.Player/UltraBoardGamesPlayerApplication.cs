namespace Qwirkle.UltraBoardGames.Player;

public class UltraBoardGamesPlayerApplication
{
    private readonly ILogger _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly BotUseCase _botUseCase;
    private readonly GameScraper _scraper;
    private readonly Coordinates _originCoordinates = Coordinates.From(25, 25);

    public UltraBoardGamesPlayerApplication(ILogger<UltraBoardGamesPlayerApplication> logger, CoreUseCase coreUseCase, BotUseCase botUseCase, GameScraper gameScraper)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
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
        while (true)
        {
            board = GetBoardAfterOpponentPlay(board);
            _scraper.TakeScreenShot();
            gameStatus = _scraper.GetGameStatus();
            if (gameStatus != GameStatus.InProgress) break;
            _ = _scraper.GetTilesOnBag();
            var playerPoints = _scraper.GetPlayerPoints();
            var opponentPoints = _scraper.GetOpponentPoints();
            var tilesOnPlayer = _scraper.GetTilesOnPlayer();
            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };

            _coreUseCase.Game = new Game(0, board, players, false);

            var tilesToPlay = board.Tiles.Count > 0
                ? _botUseCase.GetBestMove(bot, board)
                : _botUseCase.GetBestMove(bot, board, _originCoordinates);

            List<TileOnBoard> otherTilesToPlay;
            var copyBoard = Board.From(board.Tiles);
            var tilesToPlayStillHere = new List<TileOnBoard>();
            var tilesToPlayArranged = new List<TileOnBoard>();
            tilesToPlayStillHere.AddRange(tilesToPlay);
            do
            {
                var firstTilesToPlay = copyBoard.Tiles.Count > 0
                    ? tilesToPlayStillHere.Where(tile => copyBoard.IsIsolatedTile(tile)).ToList()
                    : tilesToPlayStillHere.Where(tile => tile.Coordinates == _originCoordinates).ToList();
                otherTilesToPlay = tilesToPlayStillHere.Except(firstTilesToPlay).ToList();
                tilesToPlayArranged.AddRange(firstTilesToPlay);
                if (otherTilesToPlay.Count == 0) break;
                copyBoard.Tiles.AddRange(firstTilesToPlay);
                tilesToPlayStillHere = otherTilesToPlay;
            } while (otherTilesToPlay.Count != 0);

            _scraper.Play(tilesToPlayArranged);
            _scraper.TakeScreenShot();
        }
        _scraper.CloseEndWindow();
        LogEndGame(gameStatus);
    }

    private Board GetBoardAfterOpponentPlay(Board board)
    {
        var tilesOnBoard = board.Tiles;
        var timeOut = DateTime.UtcNow.AddSeconds(4);
        while (true)
        {
            if (DateTime.UtcNow > timeOut) break;
            var tilesOnBoardUpdated = _scraper.GetTilesOnBoard();
            if (tilesOnBoardUpdated.Count < tilesOnBoard.Count || tilesOnBoardUpdated.Count == 0)
            {
                Task.Delay(500).Wait();
                continue;
            }
            tilesOnBoard = tilesOnBoardUpdated;
            break;
        }
        return Board.From(tilesOnBoard);
    }

    private void LogStartApplication() => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
    private void LogStartGame() => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
    private void LogEndGame(GameStatus gameStatus) => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", $"{gameStatus}", DateTime.UtcNow);
    private void LogEndApplication() => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);
    private static Domain.Entities.Player Player(int playerPoints, List<TileOnPlayer> tilesOnPlayer, bool isTurn) => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}