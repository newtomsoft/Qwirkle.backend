namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo Role = "Admin"
[Route("[controller]")]
public class AdminController : ControllerBase
{
    private readonly InfoUseCase _infoUseCase;

    public AdminController(InfoUseCase infoUseCase) => _infoUseCase = infoUseCase;


    [HttpGet("Player/{playerId:int}")]
    public ActionResult GetPlayerById(int playerId) => new ObjectResult(_infoUseCase.GetPlayer(playerId));


    [HttpGet("AllUsersIds")]
    public ActionResult GetAllUsersId() => new ObjectResult(_infoUseCase.GetAllUsersId());


    [HttpGet("GamesByUserId/{userId:int}")]
    public ActionResult GetGamesByUserId(int userId) => new ObjectResult(_infoUseCase.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult GetGamesIdsContainingPlayers() => new ObjectResult(_infoUseCase.GetGamesIdsContainingPlayers());


    [HttpGet("Game/{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_infoUseCase.GetGameForSuperUser(gameId));
}