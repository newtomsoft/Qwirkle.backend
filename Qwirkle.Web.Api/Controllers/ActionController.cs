namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly UserService _userService;
    private readonly BotService _botService;
    private readonly ILogger<ActionController> _logger;
    private readonly INotification _notification;
    private readonly InfoService _infoService;
    private readonly CoreService _coreService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public ActionController(CoreService coreService, InfoService infoService, UserManager<UserDao> userManager, INotification notification, UserService userService, BotService botService, ILogger<ActionController> logger)
    {
        _coreService = coreService;
        _infoService = infoService;
        _userManager = userManager;
        _notification = notification;
        _userService = userService;
        _botService = botService;
        _logger = logger;
    }


    [HttpPost("PlayTiles/")]
    public async Task<ActionResult> PlayTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var playReturn = _coreService.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard()));
        if (playReturn.Code == ReturnCode.Ok) await NotifyNextPlayerAndPlayIfBotAsync(gameId);
        return new ObjectResult(playReturn);
    }


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult PlayTilesSimulation(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreService.TryPlayTilesSimulation(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }

    [HttpPost("SwapTiles/")]
    public async Task<ActionResult> SwapTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var swapTilesReturn = _coreService.TrySwapTiles(playerId, tiles.Select(t => t.ToTile()));
        if (swapTilesReturn.Code == ReturnCode.Ok) await NotifyNextPlayerAndPlayIfBotAsync(gameId);
        return new ObjectResult(swapTilesReturn);
    }

    [HttpPost("SkipTurn/")]
    public async Task<ActionResult> SkipTurn(SkipTurnViewModel skipTurnViewModel)
    {
        var playerId = _infoService.GetPlayerId(skipTurnViewModel.GameId, UserId);
        var skipTurnReturn = _coreService.TrySkipTurn(playerId);
        if (skipTurnReturn.Code == ReturnCode.Ok) await NotifyNextPlayerAndPlayIfBotAsync(skipTurnViewModel.GameId);
        return new ObjectResult(skipTurnReturn);
    }

    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreService.TryArrangeRack(playerId, tiles.Select(t => t.ToTile())));
    }

    private async Task NotifyNextPlayerAndPlayIfBotAsync(int gameId)
    {
        var nextPlayerId = _infoService.GetPlayerIdTurn(gameId);
        if (nextPlayerId == 0) return;

        _notification?.SendPlayerIdTurn(gameId, nextPlayerId);
        await _botService.PlayBotsAsync(gameId);
    }
}