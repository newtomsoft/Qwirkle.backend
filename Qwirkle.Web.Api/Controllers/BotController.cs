using System.Reflection;
using NLog;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
[Route("Bot")]
public class BotController : ControllerBase
{
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");
    public BotController(CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _coreUseCase = coreUseCase;
        _userManager = userManager;
    }


    [HttpGet("PossibleMoves/{gameId:int}")]
    public ActionResult ComputeDoableMoves(int gameId)
    {
        _logger.Info($"userId:{_userId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_coreUseCase.ComputeDoableMoves(gameId, _userId));
    }
}