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

    [HttpGet("AllUsersIds")]
    public ActionResult<int> GetAllUsersId() => new ObjectResult(_coreUseCase.GetAllUsersId());


    [HttpPost("GamesByUserId/{userId:int}")]
    public ActionResult<int> GetGamesByUserId(int userId) => new ObjectResult(_coreUseCase.GetUserGames(userId));


    [HttpGet("GamesIds")]
    public ActionResult<int> GetGamesIdsContainingPlayers() => new ObjectResult(_coreUseCase.GetGamesIdsContainingPlayers());
}