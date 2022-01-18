namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class InstantGameController : ControllerBase
{
    private readonly ILogger<InstantGameController> _logger;
    private readonly INotification _notification;
    private readonly UserManager<UserDao> _userManager;
    private readonly InstantGameService _instantGameService;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public InstantGameController(ILogger<InstantGameController> logger, INotification notification, UserManager<UserDao> userManager, InstantGameService instantGameService)
    {
        _logger = logger;
        _notification = notification;
        _userManager = userManager;
        _instantGameService = instantGameService;
    }

    [HttpGet("Join/{playersNumberForStartGame:int}")]
    public ActionResult JoinInstantGame(int playersNumberForStartGame)
    {
        _logger?.LogInformation("JoinInstantGame with {playersNumber}", playersNumberForStartGame);
        var usersIds = _instantGameService.JoinInstantGame(UserId, playersNumberForStartGame);
        if (usersIds.Count != playersNumberForStartGame) return new ObjectResult($"waiting for {playersNumberForStartGame - usersIds.Count} player(s)");

        _notification.SendInstantGameStarted(playersNumberForStartGame); //TODO same thing for 1 player join to show progress ui
        return RedirectToAction("CreateInstantGame", "Game", new { serializedUsersIds = JsonConvert.SerializeObject(usersIds) });
    }
}
