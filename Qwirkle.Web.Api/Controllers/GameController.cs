namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(CoreService coreService, InfoService infoService, UserManager<UserDao> userManager, ILogger<GameController> logger)
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
        if (!usersIds.Contains(UserId)) return BadRequest("user not in the game");
        var gameId = _coreService.CreateGameWithUsersIds(usersIds);
        return Ok(gameId);
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId)
    {
        var game = _infoService.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId);
        return game is null ? BadRequest() : Ok(game);
    }


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => Ok(_infoService.GetUserGames(UserId));
}
