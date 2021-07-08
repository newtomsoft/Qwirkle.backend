using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.Ports;
using Qwirkle.Web.Api.VueModels;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    [ApiController]
    [Route("Games")]
    public class GamesController : ControllerBase
    {
        private ILogger<GamesController> Logger { get; }
        private ICoreUseCase CoreUseCase { get; }

        public GamesController(ILogger<GamesController> logger, ICoreUseCase coreUseCase)
        {
            Logger = logger;
            CoreUseCase = coreUseCase;
        }

        [HttpPost("")]
        public ActionResult<int> CreateGame(List<int> usersIds)
        {
            Logger.LogInformation($"CreateGame with {usersIds}");
            var players = CoreUseCase.CreateGame(usersIds);
            return new ObjectResult(players);
        }

        [HttpPost("Get")]
        public ActionResult<int> GetGame(List<int> gameId)
        {
            var game = CoreUseCase.GetGame(gameId[0]);
            return new ObjectResult(game);
        }

        [HttpGet("Players/{playerId}")]
        public ActionResult<int> GetPlayer(int playerId)
        {
            var player = CoreUseCase.GetPlayer(playerId);
            return new ObjectResult(player);
        }

        [HttpPost("PlayTiles/")]
        public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
            var playreturn = CoreUseCase.TryPlayTiles(tiles[0].PlayerId, tilesToPlay);
            return new ObjectResult(playreturn);
        }

        [HttpPost("SwapTiles/")]
        public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
        {
            var tilesIdsToChange = new List<int>();
            tiles.ForEach(t => tilesIdsToChange.Add(t.TileId));
            var swapTilesReturn = CoreUseCase.TrySwapTiles(tiles[0].PlayerId, tilesIdsToChange);
            return new ObjectResult(swapTilesReturn);
        }
    }
}
