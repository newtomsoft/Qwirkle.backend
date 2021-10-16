using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Qwikle.SignalR;
using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.UsesCases;
using Qwirkle.Web.Api.VueModels;
using System.Collections.Generic;

namespace Qwirkle.Web.Api.Controllers
{

    [ApiController]
    [Route("Games")]

    public class GamesController : ControllerBase
    {
        private readonly IHubContext<HubQwirkle> _hubContextQwirkle;

        private ILogger<GamesController> Logger { get; }
        private CoreUseCase CoreUseCase { get; }

        public GamesController(ILogger<GamesController> logger, CoreUseCase coreUseCase, IHubContext<HubQwirkle> hubContextQwirkle)
        {
            Logger = logger;
            CoreUseCase = coreUseCase;
            _hubContextQwirkle = hubContextQwirkle;
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
            var playerId = tiles[0].PlayerId;
            var playReturn = CoreUseCase.TryPlayTiles(playerId, tilesToPlay);
            if (playReturn.Code == PlayReturnCode.Ok)
            {
                int gameId = playReturn.GameId;
                SendTilesPlayed(gameId, playerId, playReturn.TilesPlayed);
                SendPlayerIdTurn(gameId, CoreUseCase.GetPlayerIdToPlay(gameId));
            }
            return new ObjectResult(playReturn);
        }

        [HttpPost("PlayTilesSimulation/")]
        public ActionResult<int> PlayTilesSimulation(List<TileViewModel> tiles)
        {
            var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToPlay.Add((t.TileId, t.X, t.Y)));
            var playerId = tiles[0].PlayerId;
            var playReturn = CoreUseCase.TryPlayTilesSimulation(playerId, tilesToPlay);
            return new ObjectResult(playReturn);
        }

        [HttpPost("SwapTiles/")]
        public ActionResult<int> SwapTiles(List<TileViewModel> tiles)
        {
            var tilesIdsToChange = new List<int>();
            tiles.ForEach(t => tilesIdsToChange.Add(t.TileId));
            var swapTilesReturn = CoreUseCase.TrySwapTiles(tiles[0].PlayerId, tilesIdsToChange);
            if (swapTilesReturn.Code == PlayReturnCode.Ok)
            {
                int gameId = swapTilesReturn.GameId;
                SendTilesSwaped(gameId, tiles[0].PlayerId);
                SendPlayerIdTurn(gameId, CoreUseCase.GetPlayerIdToPlay(gameId));
            }
            return new ObjectResult(swapTilesReturn);
        }

        [HttpPost("ArrangeRack/")]
        public ActionResult<int> ArrangeRack(List<TileViewModel> tiles)
        {
            var tilesToArrange = new List<(int tileId, sbyte x, sbyte y)>();
            tiles.ForEach(t => tilesToArrange.Add((t.TileId, t.X, t.Y)));
            var playerId = tiles[0].PlayerId;
            var arrangeRackReturn = CoreUseCase.TryArrangeRack(playerId, tilesToArrange);
            return new ObjectResult(arrangeRackReturn);
        }

        [HttpPost("SkipTurn/")]
        public ActionResult<int> SkipTurn(PlayerViewModel player)
        {
            var playerId = player.Id;
            var skipTurnReturn = CoreUseCase.TrySkipTurn(playerId);
            if (skipTurnReturn.Code == PlayReturnCode.Ok)
            {
                int gameId = skipTurnReturn.GameId;
                SendTurnSkipped(gameId, playerId);
                SendPlayerIdTurn(gameId, CoreUseCase.GetPlayerIdToPlay(gameId));
            }
            return new ObjectResult(skipTurnReturn);
        }

        [HttpPost("Winners")]
        public ActionResult<int> Winners(List<int> gamesId)
        {
            int gameId = gamesId[0];
            var winnersPlayersIds = CoreUseCase.GetWinnersPlayersId(gameId);
            if (winnersPlayersIds is null)
                return null;

            SendGameOver(gameId, winnersPlayersIds);
            return new ObjectResult(winnersPlayersIds);
        }

        private void SendTilesPlayed(int gameId, int playerId, List<TileOnBoard> tilesOnBoardPlayed) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesPlayed", playerId, tilesOnBoardPlayed);
        private void SendTilesSwaped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTilesSwaped", playerId);
        private void SendTurnSkipped(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveTurnSkipped", playerId);
        private void SendPlayerIdTurn(int gameId, int playerId) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceivePlayerIdTurn", playerId);
        private void SendGameOver(int gameId, List<int> winnersPlayersIds) => _hubContextQwirkle.Clients.Group(gameId.ToString()).SendAsync("ReceiveGameOver", winnersPlayersIds);
    }
}
