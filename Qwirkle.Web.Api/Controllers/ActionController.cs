namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly CoreUseCase _coreUseCase;
    public ActionController(CoreUseCase coreUseCase) => _coreUseCase = coreUseCase;


    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles) => new ObjectResult(_coreUseCase.TryPlayTiles(tiles.First().PlayerId, tiles.Select(t => (t.TileId, t.X, t.Y))));


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles) => new ObjectResult(_coreUseCase.TryPlayTilesSimulation(tiles.First().PlayerId, tiles.Select(t => (t.TileId, t.X, t.Y))));


    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles) => new ObjectResult(_coreUseCase.TrySwapTiles(tiles.First().PlayerId, tiles.Select(t => t.TileId)));


    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(PlayerViewModel player) => new ObjectResult(_coreUseCase.TrySkipTurn(player.Id));


    [HttpPost("ArrangeRack/")]
    public ActionResult<int> ArrangeRack(List<TileViewModel> tiles) => new ObjectResult(_coreUseCase.TryArrangeRack(tiles.First().PlayerId, tiles.Select(t => (t.TileId, t.X, t.Y))));
}