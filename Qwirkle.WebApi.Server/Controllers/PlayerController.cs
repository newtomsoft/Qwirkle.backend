namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/Player")]
public class PlayerController : ControllerBase
{
    private readonly InfoService _infoService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public PlayerController(InfoService infoService, UserManager<UserDao> userManager)
    {
        _infoService = infoService;
        _userManager = userManager;
    }

    [Obsolete("method soon to be discontinued")]
    [HttpGet("{playerId:int}")]
    public ActionResult GetById(int playerId) => new ObjectResult(_infoService.GetPlayer(playerId));

    [HttpGet("ByGameId/{gameId:int}")]
    public ActionResult GetByGameId(int gameId) => new ObjectResult(_infoService.GetPlayer(gameId, UserId));


    [HttpGet("PlayerIdByGameId/{gameId:int}")]
    public ActionResult GetIdByGameId(int gameId) => new ObjectResult(_infoService.GetPlayer(gameId, UserId).Id);


    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => new ObjectResult(_infoService.GetPlayerNameTurn(gameId));


    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => new ObjectResult(_infoService.GetPlayerIdTurn(gameId));


    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => new ObjectResult(_infoService.GetWinnersPlayersId(gameId));
}