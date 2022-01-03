namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
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
    public ActionResult CreateGame(HashSet<string> usersNames)
    {
        var usersIdsList = new List<int> { UserId };
        usersIdsList.AddRange(usersNames.Select(userName => _infoUseCase.GetUserId(userName)));
        usersIdsList.RemoveAll(id => id == 0);
        var usersIds = new HashSet<int>(usersIdsList);
        _logger?.LogInformation("CreateGame with users {usersIds}", usersIds);
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }

    [HttpPost("NewRandom")]
    public ActionResult CreateRandomGame()
    {
        var usersIds = new HashSet<int> { UserId };
        //todo : add user waiting or wait...
        _logger?.LogInformation("CreateGame with {usersIds}", usersIds);
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_infoUseCase.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId));


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => new ObjectResult(_infoUseCase.GetUserGames(UserId));
}
