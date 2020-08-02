using Microsoft.EntityFrameworkCore;
using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.PlayerContext.Entities;
using Qwirkle.Core.PlayerContext.Entities.Player;
using Qwirkle.Core.PlayerContext.Ports;
using Qwirkle.Infra.Persistance.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Infra.Persistance.Adapters
{
    public class PlayerPersistanceAdapter : IPlayerPersistance
    {
        private DefaultDbContext DbContext { get; }


        public PlayerPersistanceAdapter(DefaultDbContext defaultDbContext)
        {
            DbContext = defaultDbContext;
        }

        
        public Player GetPlayer(int playerId)
           => PlayerPersistanceToPlayer(DbContext.Players.Where(p => p.Id == playerId).FirstOrDefault());

        private Player PlayerPersistanceToPlayer(PlayerPersistance playerPersistance)
        {
            var player = new Player { Id = playerPersistance.Id, GameId = playerPersistance.GameId, GameTurn = playerPersistance.GameTurn, GamePosition = playerPersistance.GamePosition, Points = playerPersistance.Points };
            var tilesOnPlayer = DbContext.TilesOnPlayer.Where(tp => tp.PlayerId == playerPersistance.Id).Include(t => t.Tile).ToList();
            var tiles = new List<Tile>();
            tilesOnPlayer.ForEach(tp => tiles.Add(TileOnPlayerPersistanceToTile(tp)));
            player.Tiles = tiles;
            return player;
        }
        private Tile TileOnPlayerPersistanceToTile(TileOnPlayerPersistance tileOnPlayer)
         => new Tile(tileOnPlayer.TileId, tileOnPlayer.Tile.Color, tileOnPlayer.Tile.Form);


    }
}
