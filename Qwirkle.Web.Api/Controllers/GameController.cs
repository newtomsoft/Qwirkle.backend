namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;

    public GameController(ILogger<GameController> logger, CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _userManager = userManager;
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


    [HttpGet("UserGames")]
    public ActionResult<int> GetUserGames() => new ObjectResult(_coreUseCase.GetUserGames(int.Parse(_userManager.GetUserId(User) ?? "0")));


    [HttpGet("GamesIds")]
    public ActionResult<int> GetGamesIdsContainingPlayers() => new ObjectResult(_coreUseCase.GetGamesIdsContainingPlayers());
}
