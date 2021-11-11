using Qwirkle.Core.UseCases;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("Action")]
public class ActionController : ControllerBase
{
    private readonly IHubContext<HubQwirkle> _hubContextQwirkle;
    private readonly ILogger<ActionController> _logger;
    private readonly CoreUseCase _coreUseCase;

    public ActionController(ILogger<ActionController> logger, CoreUseCase coreUseCase, IHubContext<HubQwirkle> hubContextQwirkle)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _hubContextQwirkle = hubContextQwirkle;
    }

    [HttpPost("PlayTiles/")]
    public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
        tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
        var playerId = tiles[0].PlayerId;
        var playReturn = _coreUseCase.TryPlayTiles(playerId, tilesToPlay);
        if (playReturn.Code == PlayReturnCode.Ok)
        {
            int gameId = playReturn.GameId;
            SendTilesPlayed(gameId, playerId, playReturn.Points, playReturn.TilesPlayed);
            SendPlayerIdTurn(gameId, _coreUseCase.GetPlayerIdToPlay(gameId));
        }
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
        if (swapTilesReturn.Code == PlayReturnCode.Ok)
        {
            int gameId = swapTilesReturn.GameId;
            SendTilesSwapped(gameId, tiles[0].PlayerId);
            SendPlayerIdTurn(gameId, _coreUseCase.GetPlayerIdToPlay(gameId));
        }
        return new ObjectResult(swapTilesReturn);
    }

    [HttpPost("SkipTurn/")]
    public ActionResult<int> SkipTurn(PlayerViewModel player)
    {
        var playerId = player.Id;
        var skipTurnReturn = _coreUseCase.TrySkipTurn(playerId);
        if (skipTurnReturn.Code == PlayReturnCode.Ok)
        {
            int gameId = skipTurnReturn.GameId;
            SendTurnSkipped(gameId, playerId);
            SendPlayerIdTurn(gameId, _coreUseCase.GetPlayerIdToPlay(gameId));
        }
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

    private void SendTilesPlayed(int gameId, int playerId, int scoredPoints, List<TileOnBoard> tilesOnBoardPlayed) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesPlayed", playerId, scoredPoints, tilesOnBoardPlayed);
    private void SendTilesSwapped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesSwapped", playerId);
    private void SendTurnSkipped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTurnSkipped", playerId);
    private void SendPlayerIdTurn(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceivePlayerIdTurn", playerId);
}