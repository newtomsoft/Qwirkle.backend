using Qwirkle.Core.UseCases;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;

    public GameController(ILogger<GameController> logger, CoreUseCase coreUseCase)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
    }

    [HttpPost("New")]
    public ActionResult<int> CreateGame(List<int> usersIds)
    {
        _logger.LogInformation($"CreateGame with {usersIds}");
        var players = _coreUseCase.CreateGame(usersIds);
        return new ObjectResult(players);
    }

    [HttpPost("")]
    public ActionResult<int> GetGame(List<int> gameId)
    {
        var game = _coreUseCase.GetGame(gameId[0]);
        return new ObjectResult(game);
    }

    [HttpPost("GamesByUserId/{userId}")]
    public ActionResult<int> GetGamesByUserId(int userId)
    {
        var gamesId = _coreUseCase.GetUserGames(userId);
        return new ObjectResult(gamesId);
    }

    [HttpGet("GamesIds")]
    public ActionResult<int> GetGamesIdsContainingPlayers()
    {
        var listGameId = _coreUseCase.GetGamesIdsContainingPlayers();
        return new ObjectResult(listGameId);
    }
}
