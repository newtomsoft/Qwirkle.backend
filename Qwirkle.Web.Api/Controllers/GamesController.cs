using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.GameContext.Entities;
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
        private IRequestComplianceService RequestComplianceService { get; }
        private IRequestGameService RequestGameService { get; }
        private IRequestPlayerService RequestPlayerService { get; }

        public GamesController(ILogger<GamesController> logger, IRequestComplianceService requestComplianceService, IRequestGameService requestGameService, IRequestPlayerService requestPlayerService)
        {
            Logger = logger;
            RequestComplianceService = requestComplianceService;
            RequestGameService = requestGameService;
            RequestPlayerService = requestPlayerService;
        }

        [HttpPost("")]
        public ActionResult<int> CreateGame(List<int> usersIds)
        {
            Logger.LogInformation($"CreateGame with {usersIds}");
            var players = RequestComplianceService.CreateGame(usersIds);
            return new ObjectResult(players);
        }


        [HttpGet("{gameId}")]
        public ActionResult<int> GetGame(int gameId)
        {
            Logger.LogInformation("controller call");

            var game = RequestGameService.GetGame(gameId);

            return new ObjectResult(game);
        }

        [HttpGet("Players/{playerId}")]
        public ActionResult<int> GetPlayer(int playerId)
        {
            Logger.LogInformation("controller call");

            var player = RequestPlayerService.GetPlayer(playerId);

            return new ObjectResult(player);
        }

        [HttpPost("PlayTiles/")]
        public ActionResult<int> PlayTiles(List<TileVM> tiles)
        {
            Logger.LogInformation("controller call");

            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
            var playreturn = RequestComplianceService.PlayTiles(tiles[0].PlayerId, tilesToPlay);

            return new ObjectResult(playreturn);
        }

    }
}
