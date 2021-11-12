namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _userManager = userManager;
    }


    [HttpPost("New")]
    public ActionResult<int> CreateGame(List<int> usersIds)
    {
        if (UserId == 0) return NotFound();
        usersIds.Add(UserId);
        _logger.LogInformation($"CreateGame with {usersIds}");
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult<int> GetGame(int gameId) => new ObjectResult(_coreUseCase.GetGame(gameId));


    [HttpGet("UserGames")]
    public ActionResult<int> GetUserGames() => new ObjectResult(_coreUseCase.GetUserGames(UserId));
}
