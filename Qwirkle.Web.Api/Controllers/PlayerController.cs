namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly CoreUseCase _useCase;
    public PlayerController(CoreUseCase useCase) => _useCase = useCase;

    [Obsolete]
    [HttpGet("{playerId:int}")]
    public ActionResult GetById(int playerId) => new ObjectResult(_useCase.GetPlayer(playerId));

    [HttpGet("{gameId}/{userId:int}")]
    public ActionResult GetByGameIdUserId(int gameId, int userId) => new ObjectResult(_useCase.GetPlayer(gameId, userId));

    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => new ObjectResult(_useCase.GetPlayerNameTurn(gameId));

    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => new ObjectResult(_useCase.GetPlayerIdTurn(gameId));

    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => new ObjectResult(_useCase.GetWinnersPlayersId(gameId));
}