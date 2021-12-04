namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly InfoUseCase _infoUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreUseCase coreUseCase, InfoUseCase infoUseCase, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _infoUseCase = infoUseCase;
        _userManager = userManager;
    }


    [HttpPost("New")]
    public ActionResult CreateGame(List<int> usersIds)
    {
        usersIds.Add(UserId);
        _logger.LogInformation("CreateGame with {usersIds}", usersIds);
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_infoUseCase.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId));


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => new ObjectResult(_infoUseCase.GetUserGames(UserId));
}
