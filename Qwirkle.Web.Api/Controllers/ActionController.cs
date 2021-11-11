namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly ILogger<ActionController> _logger;
    private readonly CoreUseCase _coreUseCase;

    public ActionController(ILogger<ActionController> logger, CoreUseCase coreUseCase)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
    }

    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
        tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
        var playerId = tiles[0].PlayerId;
        var playReturn = _coreUseCase.TryPlayTiles(playerId, tilesToPlay);
        return new ObjectResult(playReturn);
    }

    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
        tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
        var playerId = tiles[0].PlayerId;
        var playReturn = _coreUseCase.TryPlayTilesSimulation(playerId, tilesToPlay);
        return new ObjectResult(playReturn);
    }

    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
    {
        var tilesIdsToChange = new List<int>();
        tiles.ForEach(t => tilesIdsToChange.Add(t.TileId));
        var swapTilesReturn = _coreUseCase.TrySwapTiles(tiles[0].PlayerId, tilesIdsToChange);
        return new ObjectResult(swapTilesReturn);
    }

    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(PlayerViewModel player)
    {
        var playerId = player.Id;
        var skipTurnReturn = _coreUseCase.TrySkipTurn(playerId);
        return new ObjectResult(skipTurnReturn);
    }

    [HttpPost("ArrangeRack/")]
    public ActionResult<int> ArrangeRack(List<TileViewModel> tiles)
    {
        var tilesToArrange = new List<(int tileId, sbyte x, sbyte y)>();
        tiles.ForEach(t => tilesToArrange.Add((t.TileId, t.X, t.Y)));
        var playerId = tiles[0].PlayerId;
        var arrangeRackReturn = _coreUseCase.TryArrangeRack(playerId, tilesToArrange);
        return new ObjectResult(arrangeRackReturn);
    }
}