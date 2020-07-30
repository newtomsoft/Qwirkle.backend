using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComplianceController : ControllerBase
    {
        private ILogger<BagController> Logger { get; }
        private IRequestComplianceService IRequestComplianceService { get; }

        public ComplianceController(ILogger<BagController> logger, IRequestComplianceService iRequestComplianceService)
        {
            Logger = logger;
            IRequestComplianceService = iRequestComplianceService;
        }

        [HttpGet("{playerId}/PlayTiles")]
        public int PlayTiles(int playerId) // todo complete
        {
            Logger.LogInformation("controller call");


            (int tileId, sbyte x, sbyte y) tuple1 = (1, -3, 4);
            (int tileId, sbyte x, sbyte y) tuple2 = (2, -3, 5);
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { tuple1, tuple2 };


            return IRequestComplianceService.PlayTiles(playerId, tilesToPlay);
        }
    }
}
