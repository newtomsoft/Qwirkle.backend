using System.Reflection;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
[Route("Bot")]
public class BotController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly UserManager<UserDao> _userManager;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");
    public BotController(BotUseCase botUseCase, UserManager<UserDao> userManager)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
    }


    [HttpGet("PossibleMoves/{gameId:int}")]
    public ActionResult ComputeDoableMoves(int gameId)
    {
        _logger.Info($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_botUseCase.ComputeDoableMoves(gameId, UserId));
    }
}