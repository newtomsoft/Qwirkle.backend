namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
[Route("[controller]")]
public class BotController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly UserManager<UserDao> _userManager;
    private readonly ILogger<CoreUseCase> _logger;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public BotController(BotUseCase botUseCase, UserManager<UserDao> userManager, ILogger<CoreUseCase> logger)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
        _logger = logger;
    }


    [HttpGet("PossibleMoves/{gameId:int}")]
    public ActionResult ComputeDoableMoves(int gameId)
    {
        _logger?.LogInformation($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_botUseCase.ComputeDoableMoves(gameId, UserId));
    }
}