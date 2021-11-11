namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly CoreUseCase _useCase;
    public PlayerController(CoreUseCase useCase) => _useCase = useCase;


    [HttpGet("AllUsersIds")]
    public ActionResult<int> GetAllUsersId() => new ObjectResult(_useCase.GetAllUsersId());

    [HttpGet("Players/{playerId:int}")]
    public ActionResult<int> GetById(int playerId) => new ObjectResult(_useCase.GetPlayer(playerId));

    [HttpGet("{gameId}/{userId:int}")]
    public ActionResult<int> GetByGameIdUserId(int gameId, int userId) => new ObjectResult(_useCase.GetPlayer(gameId, userId));

    [HttpGet("GetPlayerNameTurn/{gameId:int}")]
    public ActionResult<int> GetNameTurn(int gameId) => new ObjectResult(_useCase.GetPlayerNameTurn(gameId));

    [HttpGet("PlayerIdToPlay/{gameId:int}")]
    public ActionResult<int> GetIdTurn(int gameId) => new ObjectResult(_useCase.GetPlayerIdTurn(gameId));

    [HttpGet("Winners/{gameId:int}")]
    public ActionResult<int> Winners(int gameId) => new ObjectResult(_useCase.GetWinnersPlayersId(gameId));
}