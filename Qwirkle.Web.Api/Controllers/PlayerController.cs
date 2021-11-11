namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly ILogger<PlayerController> _logger;
    private readonly CoreUseCase _coreUseCase;

    public PlayerController(ILogger<PlayerController> logger, CoreUseCase coreUseCase)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
    }

    [HttpGet("UsersIds")]
    public ActionResult<int> GetUsersId()
    {
        var usersId = _coreUseCase.GetUsersId();
        return new ObjectResult(usersId);
    }

    [HttpGet("Players/{playerId}")]
    public ActionResult<int> GetPlayer(int playerId)
    {
        var player = _coreUseCase.GetPlayer(playerId);
        return new ObjectResult(player);
    }

    [HttpGet("{gameId}/{userId}")]
    public ActionResult<int> GetPlayer(int gameId, int userId)
    {
        var player = _coreUseCase.GetPlayer(gameId, userId);
        return new ObjectResult(player);
    }

    [HttpGet("GetPlayerNameTurn/{gameId:int}")]
    public ActionResult<int> GetPlayerNameTurn(int gameId)
    {
        var playerNameTurn = _coreUseCase.GetPlayerNameTurn(gameId);
        return new ObjectResult(playerNameTurn);
    }

    [HttpGet("PlayerIdToPlay/{gameId}")]
    public ActionResult<int> GetPlayerIdToPlay(int gameId)
    {
        var playerId = _coreUseCase.GetPlayerIdToPlay(gameId);
        return new ObjectResult(playerId);
    }

    [HttpPost("Winners/")]
    public ActionResult<int> Winners(List<int> gamesId)
    {
        var gameId = gamesId[0];
        var winnersPlayersIds = _coreUseCase.GetWinnersPlayersId(gameId);
        return new ObjectResult(winnersPlayersIds);
    }
}