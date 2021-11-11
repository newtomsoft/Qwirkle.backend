namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Player")]
public class PlayerController : ControllerBase
{
    private readonly IHubContext<HubQwirkle> _hubContextQwirkle;
    private readonly ILogger<PlayerController> _logger;
    private readonly CoreUseCase _coreUseCase;

    public PlayerController(ILogger<PlayerController> logger, CoreUseCase coreUseCase, IHubContext<HubQwirkle> hubContextQwirkle)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _hubContextQwirkle = hubContextQwirkle;
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
        int gameId = gamesId[0];
        var winnersPlayersIds = _coreUseCase.GetWinnersPlayersId(gameId);
        if (winnersPlayersIds is null)
            return null;

        SendGameOver(gameId, winnersPlayersIds);
        return new ObjectResult(winnersPlayersIds);
    }

    private void SendGameOver(int gameId, List<int> winnersPlayersIds) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveGameOver", winnersPlayersIds);
}