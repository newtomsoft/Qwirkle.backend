using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Qwirkle.Domain.Entities;
using Qwirkle.Domain.UseCases;
using Qwirkle.UltraBoardGames.Player;

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

    public async Task RunAsync()
    {
        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);
        Console.WriteLine("scraping program");

        var scraper = new GameScraper();
        await scraper.AcceptPoliciesAsync();

        while (true) //todo sharpen
        {
            var tilesOnBag = scraper.GetTilesOnBag();
            var playerPoints = scraper.GetPlayerPoints();
            var opponentPoints = scraper.GetOpponentPoints();
            var tilesOnPlayer = scraper.GetTilesOnPlayer();
            var tilesOnBoard = scraper.GetTilesOnBoard();

            var board = Board.From(tilesOnBoard);
            var bot = new Player(0, 0, 0, "bot", 0, playerPoints, 0, Rack.From(tilesOnPlayer), true, false);
            var opponent = new Player(0, 0, 0, "opponent", 0, opponentPoints, 0, Rack.From(tilesOnPlayer), false, false);
            var players = new List<Player> { bot, opponent };

            _coreUseCase.Game = new Game(0, board, players, false, null);

            var move = _botUseCase.GetBestMove(bot, board);
            //todo play move
        }


        _logger.LogInformation("UltraBoardGamesPlayerApplication {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);
    }
}