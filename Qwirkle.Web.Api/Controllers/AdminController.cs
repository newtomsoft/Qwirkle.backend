namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Admin")]
public class AdminController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public AdminController(ILogger<GameController> logger, CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _userManager = userManager;
    }

    [HttpPost("GamesByUserId/{userId:int}")]
    public ActionResult<int> GetGamesByUserId(int userId) => new ObjectResult(_coreUseCase.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult<int> GetGamesIdsContainingPlayers() => new ObjectResult(_coreUseCase.GetGamesIdsContainingPlayers());
}