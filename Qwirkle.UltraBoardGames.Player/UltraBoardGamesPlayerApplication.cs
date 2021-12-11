namespace Qwirkle.UltraBoardGames.Player;

internal class UltraBoardGamesPlayerApplication
{
    private readonly ILogger _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly BotUseCase _botUseCase;
    private readonly GameScraper _scraper;
    private readonly Coordinates _originCoordinates = Coordinates.From(25, 25);

    public UltraBoardGamesPlayerApplication(ILogger<UltraBoardGamesPlayerApplication> logger, CoreUseCase coreUseCase, BotUseCase botUseCase)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _botUseCase = botUseCase;
        _scraper = new GameScraper();
        _scraper.AcceptPolicies();
    }

    public void Run()
    {
        LogStartGame();
        Console.WriteLine("scraping program");

        var gameStatus = PlayGame();

        LogEndGame(gameStatus);
    }

 
    private GameStatus PlayGame()
    {
        var board = Board.Empty();
        GameStatus gameStatus;
        while (true)
        {
            board = GetBoardAfterOpponentPlay(board);
            gameStatus = _scraper.GetGameStatus();
            if (gameStatus != GameStatus.InProgress) break;
            _ = _scraper.GetTilesOnBag();
            var playerPoints = _scraper.GetPlayerPoints();
            var opponentPoints = _scraper.GetOpponentPoints();
            var tilesOnPlayer = _scraper.GetTilesOnPlayer();

            var bot = Player(playerPoints, tilesOnPlayer, true);
            var opponent = Player(opponentPoints, tilesOnPlayer, false);
            var players = new List<Domain.Entities.Player> {bot, opponent};

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
        }
        //todo fermer popup fin de partie, rentrer nom, screenshot partie
        return gameStatus;
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
            return Board.From(tilesOnBoard);
        }
        return Board.From(tilesOnBoard);
    }

    private void LogStartGame() => _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);

    private void LogEndGame(GameStatus gameStatus)
    {
        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", $"{gameStatus}", DateTime.UtcNow);
        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);
    }

    private static Domain.Entities.Player Player(int playerPoints, List<TileOnPlayer> tilesOnPlayer, bool isTurn)
        => new(0, 0, 0, "", 0, playerPoints, 0, Rack.From(tilesOnPlayer), isTurn, false);
}