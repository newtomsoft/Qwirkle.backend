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
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult<int> GetGame(int gameId) => new ObjectResult(_coreUseCase.GetGame(gameId));


    [HttpPost("GamesByUserId/{userId:int}")]
    public ActionResult<int> GetGamesByUserId(int userId) => new ObjectResult(_coreUseCase.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult<int> GetGamesIdsContainingPlayers() => new ObjectResult(_coreUseCase.GetGamesIdsContainingPlayers());
}
