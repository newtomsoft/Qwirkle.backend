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

    [HttpGet("Join/{playersNumber:int}")]
    public ActionResult JoinInstantGame(int playersNumber)
    {
        _logger?.LogInformation("JoinInstantGame with {playersNumber}", playersNumber);
        var usersIds = _instantGameService.JoinInstantGame(UserId, playersNumber);
        if (usersIds.Count != playersNumber) return new ObjectResult($"waiting for {playersNumber - usersIds.Count} player(s)");

        //_notification.SendInstantGameCreated(usersIds);
        var serializedUsersIds = JsonConvert.SerializeObject(usersIds);
        return RedirectToAction("CreateInstantGame", "Game", new { serializedUsersIds });
    }
}
