namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
[Route("Bot")]
public class BotController : ControllerBase
{
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");
    public BotController(CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _coreUseCase = coreUseCase;
        _userManager = userManager;
    }


    [HttpGet("PossiblesMoves/{gameId:int}")]
    public ActionResult<int> GetPossiblesMoves(int gameId) => new ObjectResult(_coreUseCase.GetPossiblesMoves(gameId, _userId));





}