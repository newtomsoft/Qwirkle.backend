namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class PlayerController : ControllerBase
{
    private readonly InfoUseCase _infoUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public PlayerController(InfoUseCase infoUseCase, UserManager<UserDao> userManager)
    {
        _infoUseCase = infoUseCase;
        _userManager = userManager;
    }

    [Obsolete("method soon to be discontinued")]
    [HttpGet("{playerId:int}")]
    public ActionResult GetById(int playerId) => new ObjectResult(_infoUseCase.GetPlayer(playerId));

    [HttpGet("ByGameId/{gameId:int}")]
    public ActionResult GetByGameId(int gameId) => new ObjectResult(_infoUseCase.GetPlayer(gameId, UserId));


    [HttpGet("PlayerIdByGameId/{gameId:int}")]
    public ActionResult GetIdByGameId(int gameId) => new ObjectResult(_infoUseCase.GetPlayer(gameId, UserId).Id);


    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => new ObjectResult(_infoUseCase.GetPlayerNameTurn(gameId));


    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => new ObjectResult(_infoUseCase.GetPlayerIdTurn(gameId));


    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => new ObjectResult(_infoUseCase.GetWinnersPlayersId(gameId));
}