namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/Action")]
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
        _coreService = coreService;
        _infoService = infoService;
        _userManager = userManager;
        _notification = notification;
        _userService = userService;
        _botService = botService;
    }


    [HttpPost("PlayTiles/")]
    public ActionResult PlayTiles(List<PlayTileModel> tiles)
    {
        if (tiles.Count is 0 or > CoreService.TilesNumberPerPlayer) return BadRequest();
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var playReturn = _coreService.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard()));
        if (playReturn is { Code: ReturnCode.Ok }) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(gameId));
        return Ok(playReturn);
    }

    [HttpPost("PlayTilesSimulation/")]
    public ActionResult PlayTilesSimulation(List<PlayTileModel> tiles)
    {
        if (tiles.Count is 0 or > CoreService.TilesNumberPerPlayer) return BadRequest();
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return Ok(_coreService.TryPlayTilesSimulation(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }

    [HttpPost("SwapTiles/")]
    public ActionResult SwapTiles(List<SwapTileModel> tiles)
    {
        if (tiles.Count is 0 or > CoreService.TilesNumberPerPlayer) return BadRequest();
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        var swapTilesReturn = _coreService.TrySwapTiles(playerId, tiles.Select(t => t.Tile));
        if (swapTilesReturn is { Code: ReturnCode.Ok }) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(gameId));
        return Ok(swapTilesReturn);
    }

    [HttpPost("SkipTurn/")]
    public ActionResult SkipTurn(SkipTurnModel skipTurnViewModel)
    {
        var playerId = _infoService.GetPlayerId(skipTurnViewModel.GameId, UserId);
        var skipTurnReturn = _coreService.TrySkipTurn(playerId);
        if (skipTurnReturn is { Code: ReturnCode.Ok }) NotifyNextPlayerAndPlayIfBot(_infoService.GetGame(skipTurnViewModel.GameId));
        return Ok(skipTurnReturn);
    }

    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<ArrangeTileModel> tiles)
    {
        if (tiles.Count is 0 or > CoreService.TilesNumberPerPlayer) return BadRequest();
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return Ok(_coreService.TryArrangeRack(playerId, tiles.Select(t => t.ToTileOnRack())));
    }

    private void NotifyNextPlayerAndPlayIfBot(Game game)
    {
        var nextPlayerId = _infoService.GetPlayerIdTurn(game.Id);
        if (nextPlayerId == 0) return;

        _notification.SendPlayerIdTurn(game.Id, nextPlayerId);
        var nextPlayer = game.Players.First(p => p.Id == nextPlayerId);
        if (_userService.IsBot(nextPlayer.UserId)) _botService.Play(game, nextPlayer);
    }
}