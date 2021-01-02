using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
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

        private Player PlayerPersistenceToPlayer(PlayerModel p)
        {
            var tiles = new List<TileOnPlayer>();
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == p.Id).Include(t => t.Tile).ToList();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerModelToEntity(tp)));
            var player = new Player(p.Id, p.GameId, p.GamePosition, p.Points, tiles, p.GameTurn);
            return player;
        }

        private TileOnPlayer TileOnPlayerModelToEntity(TileOnPlayerModel t)
         => new TileOnPlayer(t.RackPosition, t.TileId, t.Tile.Color, t.Tile.Form);
    }
}
