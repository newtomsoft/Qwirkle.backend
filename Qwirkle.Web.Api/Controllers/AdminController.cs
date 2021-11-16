namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo Role = "Admin"
[Route("Admin")]
public class AdminController : ControllerBase
{
    private readonly CoreUseCase _coreUseCase;
    public AdminController(CoreUseCase coreUseCase)
    {
        _coreUseCase = coreUseCase;
    }

    [HttpGet("Player/{playerId:int}")]
    public ActionResult GetPlayerById(int playerId) => new ObjectResult(_coreUseCase.GetPlayer(playerId));


    [HttpGet("AllUsersIds")]
    public ActionResult GetAllUsersId() => new ObjectResult(_coreUseCase.GetAllUsersId());


    [HttpGet("GamesByUserId/{userId:int}")]
    public ActionResult GetGamesByUserId(int userId) => new ObjectResult(_coreUseCase.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult GetGamesIdsContainingPlayers() => new ObjectResult(_coreUseCase.GetGamesIdsContainingPlayers());


    [HttpGet("Game/{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_coreUseCase.GetGame(gameId));
}