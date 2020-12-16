using Qwirkle.Core.CommonContext;
using Qwirkle.Core.CommonContext.ValueObjects;
using Qwirkle.Core.ComplianceContext.Entities;
using Qwirkle.Core.ComplianceContext.Ports;
using Qwirkle.Core.ComplianceContext.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.ComplianceContext.Services
{
    public class ComplianceService : IRequestCompliance
    {
        private const int TILES_NUMBER_PER_PLAYER = 6;
        private const int TILES_NUMBER_FOR_A_QWIRKLE = 6;
        private const int POINTS_FOR_A_QWIRKLE = 12;


        private ICompliancePersistence PersistenceAdapter { get; }

        public Game Game { get; set; }

        public ComplianceService(ICompliancePersistence persistenceAdapter) => PersistenceAdapter = persistenceAdapter;

        public List<Player> CreateGame(List<int> usersIds)
        {
            Game = PersistenceAdapter.CreateGame(DateTime.Now);
            CreatePlayers(usersIds);
            CreateTiles();
            Game.Players.ForEach(player => PersistenceAdapter.TilesFromPlayerToBag(player, player.Tiles));
            Game.Players.ForEach(player => PersistenceAdapter.TilesFromBagToPlayer(player, TILES_NUMBER_PER_PLAYER));
            RefreshPlayers();
            SelectFirstPlayer();
            return Game.Players;
        }

        public PlayReturn PlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            Player player = GetPlayer(playerId);
            if (!IsPlayerTurn(player)) return new PlayReturn { Code = PlayReturnCode.NotPlayerTurn };

            List<Tile> tilesToPlay = GetTiles(tilesTupleToPlay);
            GetGame(player.GameId);

            if (!DoesThePlayerHaveThisTiles(player, tilesToPlay)) return new PlayReturn { Code = PlayReturnCode.PlayerDontHaveThisTile };

            PlayReturn playReturn = GetPlayReturn(tilesToPlay);
            if (playReturn.Code != PlayReturnCode.Ok) return playReturn;
            PlayAndUpdateGame(player, tilesToPlay, playReturn.Points);
            return playReturn;
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

        private void CreateTiles() => PersistenceAdapter.CreateTiles(Game.Id);

        private void CreatePlayers(List<int> usersIds)
        {
            Game.Players = new List<Player>();
            usersIds.ForEach(userId => Game.Players.Add(PersistenceAdapter.CreatePlayer(userId, Game.Id)));
            SetPositionsPlayers();
            Game.Players.ForEach(player => PersistenceAdapter.UpdatePlayer(player));
        }

        private void SetPositionsPlayers()
        {
            Game.Players = Game.Players.OrderBy(_ => Guid.NewGuid()).ToList();
            for (int i = 0; i < Game.Players.Count; i++)
                Game.Players[i].GamePosition = (byte)(i + 1);
        }

        private void RefreshPlayers()
        {
            for (int i = 0; i < Game.Players.Count; i++)
                Game.Players[i] = GetPlayer(Game.Players[i].Id);
        }

        private void SelectFirstPlayer()
        {
            var playersWithNumberCanBePlayedTiles = new Dictionary<int, int>();
            Game.Players.ForEach(p => playersWithNumberCanBePlayedTiles[p.Id] = CountTilesWhichCanBePlayed(p));
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

        public PlayReturn GetPlayReturn(List<Tile> tiles)
        {
            if (Game.Tiles.Count == 0 && tiles.Count == 1) return new PlayReturn { Code = PlayReturnCode.Ok, Points = 1, Tile = tiles[0] };

            bool AreAllTilesIsolated = true;
            foreach (var tile in tiles)
                if (IsTileIsolated(tile))
                    AreAllTilesIsolated = false;
            if (Game.Tiles.Count > 0 && AreAllTilesIsolated) return new PlayReturn { Code = PlayReturnCode.TileIsolated, Points = 0 };

            int totalPoints;
            if ((totalPoints = CountTilesMakedValidRow(tiles)) == 0) return new PlayReturn { Code = PlayReturnCode.TilesDontMakedValidRow };
            return new PlayReturn { Code = PlayReturnCode.Ok, Points = totalPoints };
        }

        private List<Tile> GetTiles(List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            List<Tile> tiles = new List<Tile>();
            foreach (var (TileId, X, Y) in tilesTupleToPlay)
            {
                Tile tile = PersistenceAdapter.GetTileById(TileId);
                tile.Coordinates = new CoordinatesInGame(X, Y);
                tiles.Add(tile);
            }
            return tiles;
        }

        private List<Tile> GetTiles(List<int> tilesIds)
        {
            List<Tile> tiles = new List<Tile>();
            tilesIds.ForEach(tileId => tiles.Add(PersistenceAdapter.GetTileById(tileId)));
            return tiles;
        }

        private Player GetPlayer(int playerId) => PersistenceAdapter.GetPlayer(playerId);

        private void GetGame(int GameId) => Game = PersistenceAdapter.GetGame(GameId);

        private void SwapAndUpdateGame(Player player, List<Tile> tilesToSwap)
        {
            SetPlayerTurn(player.Id, false);
            SetNextPlayerTurnToPlay(player.Id);
            PersistenceAdapter.TilesFromBagToPlayer(player, tilesToSwap.Count);
            PersistenceAdapter.TilesFromPlayerToBag(player, tilesToSwap);
        }

        private void PlayAndUpdateGame(Player player, List<Tile> tilesToPlay, int points)
        {
            player.Points += points;
            player.GameTurn = false;
            Game.Tiles.AddRange(tilesToPlay);
            PersistenceAdapter.UpdatePlayer(player);
            SetNextPlayerTurnToPlay(player.Id);
            PersistenceAdapter.TilesFromBagToPlayer(player, tilesToPlay.Count);
            PersistenceAdapter.TilesFromPlayerToGame(Game.Id, player.Id, tilesToPlay);
        }

        private void SetNextPlayerTurnToPlay(int playerId)
        {
            int position = Game.Players.FirstOrDefault(p => p.Id == playerId).GamePosition;
            int playersNumber = Game.Players.Count;
            int nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            Player nexPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            nexPlayer.GameTurn = true;
            PersistenceAdapter.UpdatePlayer(nexPlayer);
        }

        private int CountTilesMakedValidRow(List<Tile> tiles)
        {
            if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
                return 0;

            int tatalPoints = 0;
            int points = 0;
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

        private int CountTilesMakedValidLine(List<Tile> tiles)
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

            return (allTilesAlongReferenceTiles.Count != TILES_NUMBER_FOR_A_QWIRKLE ? allTilesAlongReferenceTiles.Count : POINTS_FOR_A_QWIRKLE);
        }

        private int CountTilesMakedValidColumn(List<Tile> tiles)
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

            return allTilesAlongReferenceTiles.Count != TILES_NUMBER_FOR_A_QWIRKLE ? allTilesAlongReferenceTiles.Count : POINTS_FOR_A_QWIRKLE;
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
            => PersistenceAdapter.IsPlayerTurn(playerId);

        private void SetPlayerTurn(int playerId, bool turn)
        {
            PersistenceAdapter.SetPlayerTurn(playerId, turn);
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
