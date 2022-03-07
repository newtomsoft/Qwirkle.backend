namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("InstantGame")]
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
    public ActionResult JoinInstantGame(int playersNumberForStartGame)
    {
        if (playersNumberForStartGame is < 2 or > 4) return BadRequest("game must have between 2 and 4 players");
        _logger.LogInformation("JoinInstantGame with {playersNumber}", playersNumberForStartGame);
        var usersNames = _instantGameService.JoinInstantGame(UserName, playersNumberForStartGame);
        if (usersNames.Count != playersNumberForStartGame)
        {
            _notification.SendInstantGameExpected(playersNumberForStartGame, UserName);
            return Ok(usersNames);
        }
        var usersIds = usersNames.Select(userName => _infoService.GetUserId(userName)).ToHashSet();
        var gameId = _coreService.CreateGameWithUsersIds(usersIds);
        _notification.SendInstantGameStarted(playersNumberForStartGame, gameId);
        return Ok(gameId);
    }
}
