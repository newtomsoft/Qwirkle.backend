using Microsoft.AspNetCore.Http;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("[controller]")]
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
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoUseCase.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreUseCase.TryPlayTiles(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }


    [HttpPost("PlayTilesSimulation/")]
    public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoUseCase.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreUseCase.TryPlayTilesSimulation(playerId, tiles.Select(t => t.ToTileOnBoard())));
    }

    [HttpPost("SwapTiles/")]
    public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoUseCase.GetPlayerId(gameId, UserId);
        return new ObjectResult(_coreUseCase.TrySwapTiles(playerId, tiles.Select(t => t.ToTile())));
    }

    [HttpGet("SkipTurn/{gameId:int}")]
    public ActionResult<int> SkipTurn(int gameId)
    {


        var playerId = _infoUseCase.GetPlayerIdTurn(gameId);
        return new ObjectResult(_coreUseCase.TrySkipTurn(playerId));
    }


    [HttpPost("ArrangeRack/")]
    public ActionResult ArrangeRack(List<TileViewModel> tiles)
    {
        if (tiles.Count == 0) return StatusCode(StatusCodes.Status400BadRequest);
        var gameId = tiles.First().GameId;
        var playerId = _infoUseCase.GetPlayerId(gameId, UserId);
        
        return new ObjectResult(_coreUseCase.TryArrangeRack(playerId, tiles.Select(t => t.ToTile())));
    }
}