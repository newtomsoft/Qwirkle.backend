using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.PlayerContext.Entities;
using Qwirkle.Core.PlayerContext.Entities.Player;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.Infra.Persistence.Models;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistence.Adapters
{
    public class PlayerPersistenceAdapter : IPlayerPersistence
    {
        private DefaultDbContext DbContext { get; }


        public PlayerPersistenceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }


        public Player GetPlayer(int playerId)
           => PlayerPersistenceToPlayer(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());

        private Player PlayerPersistenceToPlayer(PlayerPersistence playerPersistence)
        {
            var player = new Player { Id = playerPersistence.Id, GameId = playerPersistence.GameId, GameTurn = playerPersistence.GameTurn, GamePosition = playerPersistence.GamePosition, Points = playerPersistence.Points };
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerPersistence.Id).Include(t => t.Tile).ToList();
            var tiles = new List<Tile>();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerPersistenceToTile(tp)));
            player.Rack = new Rack(tiles.ToArray());
            return player;
        }
        private Tile TileOnPlayerPersistenceToTile(TileOnPlayerPersistence tileOnPlayer)
         => new Tile(tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);


    }
}
