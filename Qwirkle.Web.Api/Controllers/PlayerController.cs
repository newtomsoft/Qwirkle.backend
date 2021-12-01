namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly InfoUseCase _infoUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public PlayerController(InfoUseCase infoUseCase, UserManager<UserDao> userManager)
    {
        _infoUseCase = infoUseCase;
        _userManager = userManager;
    }


    [Obsolete]
    [HttpGet("{playerId:int}")]
    public ActionResult GetById(int playerId) => new ObjectResult(_infoUseCase.GetPlayer(playerId));

    [HttpGet("{gameId}/{userId:int}")]
    public ActionResult GetByGameIdUserId(int gameId, int userId) => new ObjectResult(_infoUseCase.GetPlayer(gameId, userId));


    [HttpGet("PlayerIdByGameId/{gameId:int}")]
    public ActionResult GetByGameId(int gameId) => new ObjectResult(_infoUseCase.GetPlayer(gameId, _userId).Id);


    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => new ObjectResult(_infoUseCase.GetPlayerNameTurn(gameId));

    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => new ObjectResult(_infoUseCase.GetPlayerIdTurn(gameId));

    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => new ObjectResult(_infoUseCase.GetWinnersPlayersId(gameId));
}