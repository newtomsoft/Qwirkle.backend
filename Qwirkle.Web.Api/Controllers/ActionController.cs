namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ActionController : ControllerBase
{
    private readonly UserService _userService;
    private readonly BotService _botService;
    private readonly INotification _notification;
    private readonly InfoService _infoService;
    private readonly CoreService _coreService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public ActionController(CoreService coreService, InfoService infoService, UserManager<UserDao> userManager, INotification notification, UserService userService, BotService botService)
    {
        _userService = userService;
        _botService = botService;
        (_coreService, _infoService, _userManager, _notification) = (coreService, infoService, userManager, notification);
    }


    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var playReturn = _coreService.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard()));
        if (playReturn.Code == ReturnCode.Ok) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(gameId));
        return new ObjectResult(playReturn);
    }


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreService.TryPlayTilesSimulation(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }

    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var swapTilesReturn = _coreService.TrySwapTiles(playerId, tiles.Select(t => t.ToTile()));
        if (swapTilesReturn.Code == ReturnCode.Ok) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(gameId));
        return new ObjectResult(swapTilesReturn);
    }

    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(SkipTurnViewModel skipTurnViewModel)
    {
        var playerId = _infoService.GetPlayerId(skipTurnViewModel.GameId, UserId);
        var skipTurnReturn = _coreService.TrySkipTurn(playerId);
        if (skipTurnReturn.Code == ReturnCode.Ok) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(skipTurnViewModel.GameId));
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

    private void NotifyNextPlayerAndPlayIfBot(Game game)
    {
        var nextPlayerId = _infoService.GetPlayerIdTurn(game.Id);
        if (nextPlayerId == 0) return;

        _notification?.SendPlayerIdTurn(game.Id, nextPlayerId);
        var nextPlayer = game.Players.First(p => p.Id == nextPlayerId);
        if (_userService.IsBot(nextPlayer.Pseudo)) _botService.Play(game, nextPlayer);
    }
}