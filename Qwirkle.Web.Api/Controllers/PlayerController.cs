namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly CoreUseCase _useCase;
    private readonly InfoUseCase _infoUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");
    
    public PlayerController(CoreUseCase useCase, InfoUseCase infoUseCase, UserManager<UserDao> userManager)
    {
        _useCase = useCase;
        _infoUseCase = infoUseCase;
        _userManager = userManager;
    }


    [Obsolete]
    [HttpGet("{playerId:int}")]
    public ActionResult GetById(int playerId) => new ObjectResult(_infoUseCase.GetPlayer(playerId));

    [HttpGet("{gameId}/{userId:int}")]
    public ActionResult GetByGameIdUserId(int gameId, int userId) => new ObjectResult(_useCase.GetPlayer(gameId, userId));


    [HttpGet("PlayerIdByGameId/{gameId:int}")]
    public ActionResult GetByGameId(int gameId) => new ObjectResult(_useCase.GetPlayer(gameId, _userId).Id);


    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => new ObjectResult(_useCase.GetPlayerNameTurn(gameId));

    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => new ObjectResult(_useCase.GetPlayerIdTurn(gameId));

    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => new ObjectResult(_useCase.GetWinnersPlayersId(gameId));
}