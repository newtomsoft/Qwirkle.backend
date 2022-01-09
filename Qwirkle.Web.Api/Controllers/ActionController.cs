namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ActionController : ControllerBase
{
    private readonly InfoService _infoService;
    private readonly CoreService _coreService;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public ActionController(CoreService coreService, InfoService infoService, UserManager<UserDao> userManager) => (_coreService, _infoService, _userManager) = (coreService, infoService, userManager);


    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreService.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard())));
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
        return new ObjectResult(_coreService.TrySwapTiles(playerId, tiles.Select(t => t.ToTile())));
    }

    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(SkipTurnViewModel skipTurnViewModel)
    {
        var playerId = _infoService.GetPlayerId(skipTurnViewModel.GameId, UserId);
        return new ObjectResult(_coreService.TrySkipTurn(playerId));
    }
    
    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoService.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreService.TryArrangeRack(playerId, tiles.Select(t => t.ToTile())));
    }
}