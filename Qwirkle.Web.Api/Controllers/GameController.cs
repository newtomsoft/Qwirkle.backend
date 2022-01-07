namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreService coreService, InfoService infoService, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreService = coreService;
        _infoService = infoService;
        _userManager = userManager;
    }


    [HttpPost("New")]
    public ActionResult CreateGame(HashSet<string> usersNames)
    {
        var usersIdsList = new List<int> { UserId };
        usersIdsList.AddRange(usersNames.Select(userName => _infoService.GetUserId(userName)));
        usersIdsList.RemoveAll(id => id == 0);
        var usersIds = new HashSet<int>(usersIdsList);
        _logger?.LogInformation("CreateGame with users {usersIds}", usersIds);
        return new ObjectResult(_coreService.CreateGame(usersIds));
    }

    [HttpPost("NewRandom")]
    public ActionResult CreateRandomGame()
    {
        var usersIds = new HashSet<int> { UserId };
        //todo : add user waiting or wait...
        _logger?.LogInformation("CreateGame with {usersIds}", usersIds);
        return new ObjectResult(_coreService.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_infoService.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId));


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => new ObjectResult(_infoService.GetUserGames(UserId));
}
