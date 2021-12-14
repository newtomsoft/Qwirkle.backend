namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly InfoUseCase _infoUseCase;
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public ActionController(CoreUseCase coreUseCase, InfoUseCase infoUseCase, UserManager<UserDao> userManager) => (_coreUseCase, _infoUseCase, _userManager) = (coreUseCase, infoUseCase, userManager);


    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _infoUseCase.GetUserId(playerId);
        return userId != UserId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _infoUseCase.GetUserId(playerId);
        return userId != UserId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryPlayTilesSimulation(playerId, tiles.Select(t => (t.Color, t.Shape, Coordinates.From(t.X, t.Y)))));
    }

    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _infoUseCase.GetUserId(playerId);
        return userId != UserId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TrySwapTiles(tiles.First().PlayerId, tiles.Select(t => (t.Color, t.Shape))));
    }


    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(PlayerViewModel player)
    {
        var userId = _infoUseCase.GetUserId(player.Id);
        return userId != UserId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TrySkipTurn(player.Id));
    }


    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<TileOnPlayerViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _infoUseCase.GetUserId(playerId);
        return userId != UserId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryArrangeRack(tiles.First().PlayerId, tiles.Select(t => (t.Color, t.Shape))));
    }
}