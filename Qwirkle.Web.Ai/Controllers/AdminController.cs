namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("Admin")]
public class AdminController : ControllerBase
{
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;

    public AdminController(CoreService coreService, InfoService infoService)
    {
        _coreService = coreService;
        _infoService = infoService;
    }


    [HttpGet("Player/{playerId:int}")]
    public ActionResult GetPlayerById(int playerId) => Ok(_infoService.GetPlayer(playerId));


    [HttpGet("AllUsersIds")]
    public ActionResult GetAllUsersId() => Ok(_infoService.GetAllUsersId());


    [HttpGet("GamesByUserId/{userId:int}")]
    public ActionResult GetGamesByUserId(int userId) => Ok(_infoService.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult GetGamesIdsContainingPlayers() => Ok(_infoService.GetGamesIdsContainingPlayers());


    [HttpGet("Game/{gameId:int}")]
    public ActionResult GetGame(int gameId) => Ok(_infoService.GetGameForSuperUser(gameId));


    [HttpGet("NewGameWithOnlyBots/{botsNumber:int}")]
    [Authorize(Roles = "Admin")]
    public ActionResult CreateOnlyBotsGame(int botsNumber)
    {
        if (botsNumber is < 2 or > 4) return BadRequest();
        var usersIds = Enumerable.Range(1, botsNumber).ToHashSet();
        var gameId = _coreService.CreateGameWithUsersIds(usersIds);
        return Ok(gameId);
    }
}