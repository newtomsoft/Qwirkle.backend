namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Player")]
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
    public ActionResult GetById(int playerId) => Ok(_infoService.GetPlayer(playerId));

    [HttpGet("ByGameId/{gameId:int}")]
    public ActionResult GetByGameId(int gameId) => Ok(_infoService.GetPlayer(gameId, UserId));


    [HttpGet("PlayerIdByGameId/{gameId:int}")]
    public ActionResult GetIdByGameId(int gameId) => Ok(_infoService.GetPlayer(gameId, UserId).Id);


    [HttpGet("NameTurn/{gameId:int}")]
    public ActionResult GetNameTurn(int gameId) => Ok(_infoService.GetPlayerNameTurn(gameId));


    [HttpGet("IdTurn/{gameId:int}")]
    public ActionResult GetIdTurn(int gameId) => Ok(_infoService.GetPlayerIdTurn(gameId));


    [HttpGet("Winners/{gameId:int}")]
    public ActionResult Winners(int gameId) => Ok(_infoService.GetWinnersPlayersId(gameId));
}