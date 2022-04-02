namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/Game")]
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
    public ActionResult CreateGame(OpponentsModel opponentsModel)
    {
        var usersIdsList = new List<int> { UserId, };
        if (!string.IsNullOrEmpty(opponentsModel.Opponent1)) usersIdsList.Add(_infoService.GetUserId(opponentsModel.Opponent1));
        if (opponentsModel.Opponent2 is not null) usersIdsList.Add(_infoService.GetUserId(opponentsModel.Opponent2));
        if (opponentsModel.Opponent3 is not null) usersIdsList.Add(_infoService.GetUserId(opponentsModel.Opponent3));
        usersIdsList.RemoveAll(id => id == 0);
        var usersIds = new HashSet<int>(usersIdsList);
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