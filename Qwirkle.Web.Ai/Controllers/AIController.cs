


namespace Qwirkle.Web.Ai.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("[controller]")]
public class AiController : ControllerBase
{
    private readonly BotAlphaQwirkle _botAlphaQwirkle;
    private readonly UserManager<UserDao> _userManager;
    private readonly ILogger<CoreService> _logger;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");


    public AiController(BotAlphaQwirkle botAlphaQwirkle, UserManager<UserDao> userManager, ILogger<CoreService> logger)
    {
        _botAlphaQwirkle = botAlphaQwirkle;
        _userManager = userManager;
        _logger = logger;

    }

    [HttpGet("BestMoves/{gameId:int}")]
    public ActionResult BestMoves(int gameId)
    {
        _logger?.LogInformation($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_botAlphaQwirkle.AlphaQwirkle(gameId));

    }

}