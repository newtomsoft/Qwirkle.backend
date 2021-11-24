using Qwirkle.Core.ValueObjects;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");
    public ActionController(CoreUseCase coreUseCase, UserManager<UserDao> userManager) => (_coreUseCase, _userManager) = (coreUseCase, userManager);


    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _coreUseCase.GetUserId(playerId);
        return userId != _userId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryPlayTiles(tiles.First().PlayerId, tiles.Select(t => (t.TileId, Coordinates.From(t.X, t.Y)))));
    }


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _coreUseCase.GetUserId(playerId);
        return userId != _userId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryPlayTilesSimulation(playerId, tiles.Select(t => (t.TileId, Coordinates.From(t.X, t.Y)))));
    }


    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _coreUseCase.GetUserId(playerId);
        return userId != _userId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TrySwapTiles(tiles.First().PlayerId, tiles.Select(t => t.TileId)));
    }


    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(PlayerViewModel player)
    {
        var userId = _coreUseCase.GetUserId(player.Id);
        return userId != _userId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TrySkipTurn(player.Id));
    }


    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<TileViewModel> tiles)
    {
        var playerId = tiles.First().PlayerId;
        var userId = _coreUseCase.GetUserId(playerId);
        return userId != _userId ? new NotFoundObjectResult("") : new ObjectResult(_coreUseCase.TryArrangeRack(tiles.First().PlayerId, tiles.Select(t => (t.TileId, Coordinates.From(t.X, t.Y)))));
    }
}