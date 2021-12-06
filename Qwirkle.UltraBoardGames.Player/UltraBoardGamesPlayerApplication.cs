namespace Qwirkle.UltraBoardGames.Player;

internal class UltraBoardGamesPlayerApplication
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly BotUseCase _botUseCase;
    public UltraBoardGamesPlayerApplication(IConfiguration configuration, ILogger<UltraBoardGamesPlayerApplication> logger, CoreUseCase coreUseCase, BotUseCase botUseCase)
    {
        _configuration = configuration;
        _logger = logger;
        _coreUseCase = coreUseCase;
        _botUseCase = botUseCase;
    }

    public void Run()
    {
        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
        Console.WriteLine("scraping program");

        var scraper = new GameScraper();
        scraper.AcceptPolicies();
        var tilesOnBoard = scraper.GetTilesOnBoard();
        var isTilesOnBoardEmpty = true;
        if (tilesOnBoard.Count == 0)
        {

            isTilesOnBoardEmpty = false;
        }

        while (true) //todo sharpen
        {
            WaitOpponentPlay();
            var tilesOnBag = scraper.GetTilesOnBag();
            var playerPoints = scraper.GetPlayerPoints();
            var opponentPoints = scraper.GetOpponentPoints();
            var tilesOnPlayer = scraper.GetTilesOnPlayer();
            while ((tilesOnBoard = scraper.GetTilesOnBoard()).Count == 0 && !isTilesOnBoardEmpty) ;

            var board = Board.From(tilesOnBoard);
            var bot = new Domain.Entities.Player(0, 0, 0, "bot", 0, playerPoints, 0, Rack.From(tilesOnPlayer), true, false);
            var opponent = new Domain.Entities.Player(0, 0, 0, "opponent", 0, opponentPoints, 0, Rack.From(tilesOnPlayer), false, false);
            var players = new List<Domain.Entities.Player> { bot, opponent };

            _coreUseCase.Game = new Game(0, board, players, false, null);

            var tilesOnBoardToPlay = _botUseCase.GetBestMove(bot, board);
            scraper.Play(tilesOnBoardToPlay);
            isTilesOnBoardEmpty = false;
        }


        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);
    }

    private void WaitOpponentPlay() => Task.Delay(1000); //todo better
}