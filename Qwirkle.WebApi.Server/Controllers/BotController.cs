namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize(Roles = "Bot")]
[Route("api/Bot")]
public class BotController : ControllerBase
{
    private readonly BotService _botService;
    private readonly UserManager<UserDao> _userManager;
    private readonly ILogger<CoreService> _logger;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public BotController(BotService botService, UserManager<UserDao> userManager, ILogger<CoreService> logger)
    {
        _botService = botService;
        _userManager = userManager;
        _logger = logger;
    }


    [HttpGet("PossibleMoves/{gameId:int}")]
    public ActionResult ComputeDoableMoves(int gameId)
    {
        _logger?.LogInformation($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_botService.ComputeDoableMoves(gameId, UserId));
    }
}