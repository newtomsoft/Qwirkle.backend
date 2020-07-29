using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComplianceController : ControllerBase
    {
        private readonly ILogger<BagController> _logger;
        private readonly IRequestComplianceService _iRequestComplianceService;

        public ComplianceController(ILogger<BagController> logger, IRequestComplianceService iRequestComplianceService)
        {
            _logger = logger;
            _iRequestComplianceService = iRequestComplianceService;
        }

        [HttpGet("{gameId}/PlayTiles")]
        public int PlayTiles(int gameId) // todo complete
        {
            var tiles = new List<Tile> { new Tile(TileColor.Purple, TileForm.Square, new CoordinatesInBoard(9, -5)) };
            Board board = new Board(gameId, tiles);
            Player player = new Player { Id = 1 };

            var tilesToPlay = new List<Tile> { new Tile(TileColor.Yellow, TileForm.Square, new CoordinatesInBoard(9, -4)) };

            return _iRequestComplianceService.PlayTiles(board, player, tilesToPlay);
        }
    }
}
