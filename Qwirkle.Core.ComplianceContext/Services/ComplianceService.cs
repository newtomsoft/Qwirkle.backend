using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Services
{
    public class ComplianceService : IRequestComplianceService
    {
        private const int TILES_NUMBER_PER_PLAYER = 6;

        private ICompliancePersistance Persistance { get; }

        public Game Game { get; set; }

        public ComplianceService(ICompliancePersistance persistance) => Persistance = persistance;

        public List<Player> CreateGame(List<int> usersIds)
        {
            Game = Persistance.CreateGame(DateTime.Now);
            List<Player> players = CreatePlayers(usersIds);
            Game.Players = players;
            CreateTiles();
            players.ForEach(player => Persistance.TilesFromPlayerToBag(player, player.Tiles));
            players.ForEach(player => Persistance.TilesFromBagToPlayer(player, TILES_NUMBER_PER_PLAYER));
            RefreshPlayers(players);
            SelectFirstPlayer(players);
            return players;
        }    

        public int PlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            Player player = GetPlayer(playerId);
            if (!IsPlayerTurn(player)) return 0;

            List<Tile> tilesToPlay = GetTiles(tilesTupleToPlay);
            GetGame(player.GameId);

            if (!DoesThePlayerHaveThisTiles(player, tilesToPlay)) return 0;

            byte points;
            if ((points = GetPlayPoints(tilesToPlay)) == 0) return 0;
            PlayAndUpdateGame(player, tilesToPlay, points);
            return points;
        }

        public bool SwapTiles(int playerId, List<int> tilesIds)
        {
            Player player = GetPlayer(playerId);
            if (!IsPlayerTurn(player)) return false;

            List<Tile> tilesToSwap = GetTiles(tilesIds);
            GetGame(player.GameId);

            if (!DoesThePlayerHaveThisTiles(player, tilesToSwap)) return false;

            SwapAndUpdateGame(player, tilesToSwap);
            return true;
        }

        private void CreateTiles() => Persistance.CreateTiles(Game.Id);

        private List<Player> CreatePlayers(List<int> usersIds)
        {
            List<Player> players = new List<Player>();
            usersIds.ForEach(userId => players.Add(Persistance.CreatePlayer(userId, Game.Id)));
            players = SetPositionsPlayers(players);
            players.ForEach(player => Persistance.UpdatePlayer(player));
            return players;
        }

        private List<Player> SetPositionsPlayers(List<Player> players)
        {
            players = players.OrderBy(_ => Guid.NewGuid()).ToList();
            for (int i = 0; i < players.Count; i++)
                players[i].GamePosition = (byte)(i + 1);

            return players;
        }

        private void RefreshPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
                players[i] = GetPlayer(players[i].Id);
        }

        private void SelectFirstPlayer(List<Player> players)
        {
            var playersWithNumberCanBePlayedTiles = new Dictionary<int, int>();
            players.ForEach(p => playersWithNumberCanBePlayedTiles[p.Id] = CountTilesWhichCanBePlayed(p));
            var playerIdToPlay = playersWithNumberCanBePlayedTiles.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
            SetPlayerTurn(playerIdToPlay, true);
        }

        private int CountTilesWhichCanBePlayed(Player player)
        {
            var tiles = player.Tiles;
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

        private bool DoesThePlayerHaveThisTiles(Player player, List<Tile> tilesToPlay)
        {
            var playerTilesId = player.Tiles.Select(t => t.Id);
            var tilesIdToPlay = tilesToPlay.Select(t => t.Id);
            return tilesIdToPlay.All(id => playerTilesId.Contains(id));
        }

        public byte GetPlayPoints(List<Tile> tiles)
        {
            if (Game.Tiles.Count == 0 && tiles.Count == 1) return 1;

            bool AreAllTilesIsolated = true;
            foreach (var tile in tiles)
                if (IsTileIsolated(tile))
                    AreAllTilesIsolated = false;
            if (Game.Tiles.Count > 0 && AreAllTilesIsolated) return 0;

            byte totalPoints;
            if ((totalPoints = CountTilesMakedValidRow(tiles)) == 0) return 0;
            return totalPoints;
        }

        private List<Tile> GetTiles(List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var (TileId, X, Y) in tilesTupleToPlay)
            {
                Tile tile = Persistance.GetTileById(TileId);
                tile.Coordinates = new CoordinatesInGame(X, Y);
                tiles.Add(tile);
            }
            return tiles;
        }

        private List<Tile> GetTiles(List<int> tilesIds)
        {
            List<Tile> tiles = new List<Tile>();
            tilesIds.ForEach(tileId => tiles.Add(Persistance.GetTileById(tileId)));
            return tiles;
        }

        private Player GetPlayer(int playerId) => Persistance.GetPlayerById(playerId);

        private void GetGame(int GameId) => Game = Persistance.GetGame(GameId);

        private void SwapAndUpdateGame(Player player, List<Tile> tilesToSwap)
        {
            SetPlayerTurn(player.Id, false);
            //player.GameTurn = false;
            //Persistance.UpdatePlayer(player);
            
            SetNextPlayerTurnToPlay(player.Id);
            Persistance.TilesFromBagToPlayer(player, tilesToSwap.Count);
            Persistance.TilesFromPlayerToBag(player, tilesToSwap);
        }

        private void PlayAndUpdateGame(Player player, List<Tile> tilesToPlay, byte points)
        {
            player.Points += points;
            player.GameTurn = false;
            Game.Tiles.AddRange(tilesToPlay);
            Persistance.UpdatePlayer(player);
            SetNextPlayerTurnToPlay(player.Id);
            Persistance.TilesFromBagToPlayer(player, tilesToPlay.Count);
            Persistance.TilesFromPlayerToGame(Game.Id, tilesToPlay);
        }

        private void SetNextPlayerTurnToPlay(int playerId)
        {
            int position = Game.Players.FirstOrDefault(p => p.Id == playerId).GamePosition;
            int playersNumber = Game.Players.Count;
            int nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            Player nexPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            nexPlayer.GameTurn = true;
            Persistance.UpdatePlayer(nexPlayer);
        }

        private byte CountTilesMakedValidRow(List<Tile> tiles)
        {
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
                return 0;

            byte tatalPoints = 0;
            byte points = 0;
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
            {
                if ((points = CountTilesMakedValidLine(tiles)) == 0) return 0;
                if (points != 1) tatalPoints += points;
                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if ((points = CountTilesMakedValidColumn(new List<Tile> { tile })) == 0) return 0;
                        if (points != 1) tatalPoints += points;
                    }
                }
            }
            if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) == tiles.Count)
            {
                if ((points = CountTilesMakedValidColumn(tiles)) == 0) return 0;
                if (points != 1) tatalPoints += points;

                if (tiles.Count > 1)
                {
                    foreach (var tile in tiles)
                    {
                        if ((points = CountTilesMakedValidLine(new List<Tile> { tile })) == 0) return 0;
                        if (points != 1) tatalPoints += points;
                    }
                }
            }
            return tatalPoints;
        }

        private byte CountTilesMakedValidLine(List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
            var tilesBetweenReference = Game.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesRight = Game.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
            var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
            allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

            var tilesLeft = Game.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
            var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
            allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return (byte)(allTilesAlongReferenceTiles.Count != 6 ? allTilesAlongReferenceTiles.Count : 12);
        }

        private byte CountTilesMakedValidColumn(List<Tile> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
            var tilesBetweenReference = Game.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesUp = Game.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
            var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
            allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

            var tilesBottom = Game.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
            var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
            allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return (byte)(allTilesAlongReferenceTiles.Count != 6 ? allTilesAlongReferenceTiles.Count : 12);
        }

        private static bool AreNumbersConsecutive(List<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

        private bool IsTileIsolated(Tile tile)
        {
            var tileRight = Game.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Right());
            var tileLeft = Game.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Left());
            var tileTop = Game.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Top());
            var tileBottom = Game.Tiles.FirstOrDefault(t => t.Coordinates == tile.Coordinates.Bottom());
            return tileRight != null || tileLeft != null || tileTop != null || tileBottom != null;
        }

        private bool IsPlayerTurn(Player player)
            => player.GameTurn;

        private bool IsPlayerTurn(int playerId)
            => Persistance.IsPlayerTurn(playerId);

        private void SetPlayerTurn(int playerId, bool turn)
        {    
            Persistance.SetPlayerTurn(playerId, turn);
            Game.Players.FirstOrDefault(p => p.Id == playerId).GameTurn = turn;
        }
    }
    public static class TileExtension
    {
        public static List<Tile> FirstConsecutives(this List<Tile> tiles, Direction direction, sbyte reference)
        {
            int diff = direction == Direction.Right || direction == Direction.Top ? -1 : 1;
            var result = new List<Tile>();
            if (tiles.Count == 0) return result;
            if ((direction == Direction.Left || direction == Direction.Right) && reference != tiles[0].Coordinates.X + diff) return result;
            if ((direction == Direction.Top || direction == Direction.Bottom) && reference != tiles[0].Coordinates.Y + diff) return result;

            result.Add(tiles[0]);
            for (int i = 1; i < tiles.Count; i++)
            {
                if ((direction == Direction.Left || direction == Direction.Right) && tiles[i - 1].Coordinates.X == tiles[i].Coordinates.X + diff && tiles[i - 1].Coordinates.Y == tiles[i].Coordinates.Y
                 || (direction == Direction.Top || direction == Direction.Bottom) && tiles[i - 1].Coordinates.Y == tiles[i].Coordinates.Y + diff && tiles[i - 1].Coordinates.X == tiles[i].Coordinates.X)
                    result.Add(tiles[i]);
                else
                    break;
            }
            return result;
        }

        public static bool AreRowByTileRespectsRules(this List<Tile> tiles)
        {
            for (int i = 0; i < tiles.Count; i++)
                for (int j = i + 1; j < tiles.Count; j++)
                    if (!tiles[i].HaveFormOrColorOnlyEqual(tiles[j])) return false;

            return true;
        }

    }
}
