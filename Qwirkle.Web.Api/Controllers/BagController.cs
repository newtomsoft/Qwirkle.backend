using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Qwirkle.Core.BagContext.Entities;
using Qwirkle.Core.BagContext.Ports;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{
    //todo : l'appel au service "métier bag" nécessite un id de bag. le mettre dans chaque appel (sauf pour la création d'un bag)
    [ApiController]
    [Route("[controller]")]
    public class BagController : ControllerBase
    {
        private readonly ILogger<BagController> _logger;
        private readonly IRequestBagService _iRequestBagService;

        public BagController(ILogger<BagController> logger, IRequestBagService iRequestBagService)
        {
            _logger = logger;
            _iRequestBagService = iRequestBagService;
        }

        [HttpGet("{gameId}/tiles/random")]
        public Tile GetRandomTile(int gameId)
        {
            _iRequestBagService.GetAllTilesOfBag(gameId);
            return _iRequestBagService.GetRandomTileOfBag(gameId);
        }

        [HttpGet("{gameId}/tiles")]
        public IEnumerable<Tile> GetTiles(int gameId)
        {
            return _iRequestBagService.GetAllTilesOfBag(gameId);
        }
    }
}
