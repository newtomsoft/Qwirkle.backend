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
    private readonly InfoService _infoService;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");
    private string UserName => _userManager.GetUserName(User) ?? string.Empty;

    public InstantGameController(ILogger<InstantGameController> logger, INotification notification, UserManager<UserDao> userManager, InstantGameService instantGameService, InfoService infoService)
    {
        _logger = logger;
        _notification = notification;
        _userManager = userManager;
        _instantGameService = instantGameService;
        _infoService = infoService;
    }

    [HttpGet("Join/{playersNumberForStartGame:int}")]
    public ActionResult JoinInstantGame(int playersNumberForStartGame)
    {
        _logger?.LogInformation("JoinInstantGame with {playersNumber}", playersNumberForStartGame);
        var usersIds = _instantGameService.JoinInstantGame(UserId, playersNumberForStartGame);
        if (usersIds.Count != playersNumberForStartGame)
        {
            _notification.SendInstantGameExpected(playersNumberForStartGame, UserName);
            return new ObjectResult($"waiting for {playersNumberForStartGame - usersIds.Count} player(s)");
        }

        _notification.SendInstantGameStarted(playersNumberForStartGame);
        return RedirectToAction("CreateInstantGame", "Game", new { serializedUsersIds = JsonConvert.SerializeObject(usersIds) });
    }
}
