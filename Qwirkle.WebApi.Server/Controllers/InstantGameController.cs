namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/InstantGame")]
public class InstantGameController : ControllerBase
{
    private readonly ILogger<InstantGameController> _logger;
    private readonly INotification _notification;
    private readonly UserManager<UserDao> _userManager;
    private readonly InstantGameService _instantGameService;
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;
    private string UserName => _userManager.GetUserName(User) ?? string.Empty;

    public InstantGameController(INotification notification, UserManager<UserDao> userManager, InstantGameService instantGameService, CoreService coreService, InfoService infoService, ILogger<InstantGameController> logger)
    {
        _logger = logger;
        _notification = notification;
        _userManager = userManager;
        _instantGameService = instantGameService;
        _coreService = coreService;
        _infoService = infoService;
    }

    [HttpGet("Join/{playersNumberForStartGame:int}")]
    public ActionResult<InstantGameModel> JoinInstantGame(int playersNumberForStartGame)
    {
        if (playersNumberForStartGame is < 2 or > 4) return BadRequest("game must have between 2 and 4 players");
        _logger.LogInformation("JoinInstantGame with {playersNumber}", playersNumberForStartGame);
        var instantGameResult = _instantGameService.JoinInstantGame(UserName, playersNumberForStartGame);
        var usersNames = instantGameResult.UsersNames;
        if (usersNames.Count != playersNumberForStartGame)
        {
            if (instantGameResult.IsAdded) _notification.SendInstantGameJoined(playersNumberForStartGame, UserName);
            return Ok(new InstantGameModel(instantGameResult.IsAdded, usersNames.ToArray()));
        }
        var usersIds = usersNames.Select(userName => _infoService.GetUserId(userName)).ToHashSet();
        var gameId = _coreService.CreateGameWithUsersIds(usersIds);
        _notification.SendInstantGameStarted(playersNumberForStartGame, gameId);
        return Ok(new InstantGameModel(instantGameResult.IsAdded, usersNames.ToArray(), gameId));
    }
}