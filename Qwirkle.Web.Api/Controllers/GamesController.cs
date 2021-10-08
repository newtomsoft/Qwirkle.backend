using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Qwikle.SignalR;
using Qwirkle.Core.Ports;
using Qwirkle.Core.UsesCases;
using Qwirkle.Web.Api.VueModels;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    
    [ApiController]
    [Route("Games")]

    public class GamesController : ControllerBase
    {
        private ILogger<GamesController> Logger { get; }
        private CoreUseCase CoreUseCase { get; }

        public GamesController(ILogger<GamesController> logger, CoreUseCase coreUseCase)
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
         
        [HttpGet("ListGameId")]
        public ActionResult<int> GetListGameIDWithPlayer()
        {
            var listGameId = CoreUseCase.GetListGameIDWithPlayer();
            return new ObjectResult(listGameId);
        }

        [HttpPost("ListNamePlayer/{gameId}")]      
        public ActionResult<int> GetListNamePlayer(int gameId)
        {
            var listName = CoreUseCase.GetListNamePlayer(gameId);
            return new ObjectResult(listName);
        }
        [HttpGet("Players/{playerId}")]
        public ActionResult<int> GetPlayer(int playerId)
        {
            var player = CoreUseCase.GetPlayer(playerId);
            return new ObjectResult(player);
        }

        [HttpGet("GetPlayerNameTurn/{gameId}")]
        public ActionResult<int> GetPlayerNameTurn(int gameId)
        {
            var playerNameTurn = CoreUseCase.GetPlayerNameTurn(gameId);
            return new ObjectResult(playerNameTurn);
        }

        [HttpGet("PlayerIdToPlay/{gameId}")]
        public ActionResult<int> GetPlayerIdToPlay(int gameId)
        {
            var playerId = CoreUseCase.GetPlayerIdToPlay(gameId);
            return new ObjectResult(playerId);
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
