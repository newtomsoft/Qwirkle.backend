using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Ports;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.Web.Api.VueModels;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    [ApiController]
    [Route("Games")]
    public class GamesController : ControllerBase
    {
        private ILogger<GamesController> Logger { get; }
        private IRequestCompliance RequestCompliance { get; }
        private IRequestGame RequestGame { get; }
        private IRequestPlayer RequestPlayer { get; }

        public GamesController(ILogger<GamesController> logger, IRequestCompliance requestCompliance, IRequestGame requestGameService, IRequestPlayer requestPlayerService)
        {
            Logger = logger;
            RequestCompliance = requestCompliance;
            RequestGame = requestGameService;
            RequestPlayer = requestPlayerService;
        }

        [HttpPost("")]
        public ActionResult<int> CreateGame(List<int> usersIds)
        {
            Logger.LogInformation($"CreateGame with {usersIds}");
            var players = RequestCompliance.CreateGame(usersIds);
            return new ObjectResult(players);
        }


        [HttpGet("{gameId}")]
        public ActionResult<int> GetGame(int gameId)
        {
            Logger.LogInformation("controller call");

            var game = RequestGame.GetGame(gameId);

            return new ObjectResult(game);
        }

        [HttpGet("Players/{playerId}")]
        public ActionResult<int> GetPlayer(int playerId)
        {
            Logger.LogInformation("controller call");

            var player = RequestPlayer.GetPlayer(playerId);

            return new ObjectResult(player);
        }

        [HttpPost("PlayTiles/")]
        public ActionResult<int> PlayTiles(List<TileViewModel> tiles)
        {
            Logger.LogInformation("controller call");

            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
            var playreturn = RequestCompliance.PlayTiles(tiles[0].PlayerId, tilesToPlay);

            return new ObjectResult(playreturn);
        }

    }
}
