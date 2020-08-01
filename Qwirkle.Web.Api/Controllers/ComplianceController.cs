using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Web.Api.VueModels;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    [ApiController]
    [Route("Games")]
    public class ComplianceController : ControllerBase
    {
        private ILogger<ComplianceController> Logger { get; }
        private IRequestComplianceService IRequestComplianceService { get; }

        public ComplianceController(ILogger<ComplianceController> logger, IRequestComplianceService iRequestComplianceService)
        {
            Logger = logger;
            IRequestComplianceService = iRequestComplianceService;
        }

        [HttpPost("")]
        public ActionResult<int> CreateGame(List<int> usersIds)
        {
            Logger.LogInformation($"CreateGame with {usersIds}");
            var players = IRequestComplianceService.CreateGame(usersIds);
            return new ObjectResult(players);
        }

        [HttpPost("PlayTiles/")]
        public ActionResult<int> PlayTiles(List<TileVM> tiles)
        {
            Logger.LogInformation("controller call");

            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
            IRequestComplianceService.PlayTiles(tiles[0].PlayerId, tilesToPlay);

            return new ObjectResult(tiles); //todo
        }
    }
}
