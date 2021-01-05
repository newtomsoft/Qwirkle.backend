using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.Entities
{
    public class Player
    {
        private bool _gameTurn;

        public int Id { get; set; }
        public string Pseudo { get; set; }
        public int GameId { get; set; }
        public int GamePosition { get; set; }
        public int Points { get; set; }
        public Rack Rack { get; set; }


        public Player(int id, int gameId, int gamePosition, int points, List<TileOnPlayer> tiles, bool turn) // todo remplacer tiles par rack
        {
            Id = id;
            GameId = gameId;
            GamePosition = gamePosition;
            Points = points;
            Rack = new Rack(tiles);
            _gameTurn = turn;
        }

        public bool IsTurn => _gameTurn;
        public void SetTurn(bool turn) => _gameTurn = turn;
        public bool HasTiles(List<int> tilesIds)
        {
            var playerTilesId = Rack.Tiles.Select(t => t.Id);
            return tilesIds.All(id => playerTilesId.Contains(id));
        }

        public int TilesNumberCanBePlayedAtGameBeginning()
        {
            var tiles = Rack.Tiles;
            int maxSameColor = 0;
            int maxSameForm = 0;
            for (int i = 0; i < tiles.Count; i++)
            {
                int sameColor = 0;
                int sameForm = 0;
                for (int j = i + 1; j < tiles.Count; j++)
                {
                    if (tiles[i].Color == tiles[j].Color && tiles[i].Form != tiles[j].Form)
                        sameColor++;
                    if (tiles[i].Color != tiles[j].Color && tiles[i].Form == tiles[j].Form)
                        sameForm++;
                }
                maxSameColor = Math.Max(maxSameColor, sameColor);
                maxSameForm = Math.Max(maxSameForm, sameForm);
            }
            return Math.Max(maxSameColor, maxSameForm) + 1;
        }
    }
}
