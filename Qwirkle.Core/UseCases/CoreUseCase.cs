﻿using Qwirkle.Core.Entities;
using Qwirkle.Core.Enums;
using Qwirkle.Core.ExtensionMethods;
using Qwirkle.Core.Ports;
using Qwirkle.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qwirkle.Core.UsesCases
{
    public class CoreUseCase
    {
        private const int TILES_NUMBER_PER_PLAYER = 6;
        private const int TILES_NUMBER_FOR_A_QWIRKLE = 6;
        private const int POINTS_FOR_A_QWIRKLE = 12;

        private IRepository RepositoryAdapter { get; }

        public Game Game { get; set; }

        public CoreUseCase(IRepository repositoryAdapter) => RepositoryAdapter = repositoryAdapter;

        public List<Player> CreateGame(List<int> usersIds)
        {
            Game = RepositoryAdapter.CreateGame(DateTime.Now);
            CreatePlayers(usersIds);
            CreateTiles();
            DealTilesToPlayers();
            RefreshPlayers();
            SelectFirstPlayer();
            return Game.Players;
        }

        public PlayReturn TryPlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            Player player = GetPlayer(playerId);
            if (!player.IsTurn) return new PlayReturn { Code = PlayReturnCode.NotPlayerTurn };

            List<TileOnBoard> tilesToPlay = GetTiles(tilesTupleToPlay);

            List<int> tilesIds = new List<int>();
            foreach (var tiles in tilesToPlay)
                tilesIds.Add(tiles.Id);

            Game = GetGame(player.GameId);

            if (!player.HasTiles(tilesIds)) return new PlayReturn { Code = PlayReturnCode.PlayerDontHaveThisTile };

            PlayReturn playReturn = GetPlayReturn(tilesToPlay);
            if (playReturn.Code != PlayReturnCode.Ok) return playReturn;
            playReturn.NewRack = PlayTiles(player, tilesToPlay, playReturn.Points);
            return playReturn;
        }

        public SwapTilesReturn TrySwapTiles(int playerId, List<int> tilesIds)
        {
            Player player = GetPlayer(playerId);
            Game = GetGame(player.GameId);
            if (!player.IsTurn) return new SwapTilesReturn { Code = PlayReturnCode.NotPlayerTurn };
            if (!player.HasTiles(tilesIds)) return new SwapTilesReturn { Code = PlayReturnCode.PlayerDontHaveThisTile };

            List<TileOnPlayer> tilesToSwap = GetPlayerTiles(tilesIds);
            var swapTilesReturn = SwapTiles(player, tilesToSwap);
            return swapTilesReturn;
        }

        private void DealTilesToPlayers()
        {
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TILES_NUMBER_PER_PLAYER; i++)
                rackPositions.Add(i);

            foreach (var player in Game.Players)
                RepositoryAdapter.TilesFromBagToPlayer(player, rackPositions);
        }

        private void CreateTiles() => RepositoryAdapter.CreateTiles(Game.Id);

        private void CreatePlayers(List<int> usersIds)
        {
            Game.Players = new List<Player>();
            usersIds.ForEach(userId => Game.Players.Add(RepositoryAdapter.CreatePlayer(userId, Game.Id)));
            SetPositionsPlayers();
            Game.Players.ForEach(player => RepositoryAdapter.UpdatePlayer(player));
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
            Game.Players.ForEach(p => playersWithNumberCanBePlayedTiles[p.Id] = p.TilesNumberCanBePlayedAtGameBeginning());
            var playerIdToPlay = playersWithNumberCanBePlayedTiles.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
            SetPlayerTurn(playerIdToPlay, true);
        }

        public PlayReturn GetPlayReturn(List<TileOnBoard> tiles)
        {
            if (Game.Board.Tiles.Count == 0 && tiles.Count == 1) return new PlayReturn { Code = PlayReturnCode.Ok, Points = 1, TilesPlayed = tiles };

            bool AreAllTilesIsolated = true;
            foreach (var tile in tiles)
                if (Game.Board.IsIsolatedTile(tile))
                    AreAllTilesIsolated = false;
            if (Game.Board.Tiles.Count > 0 && AreAllTilesIsolated) return new PlayReturn { Code = PlayReturnCode.TileIsolated, Points = 0 };

            int totalPoints;
            if ((totalPoints = CountTilesMakedValidRow(tiles)) == 0) return new PlayReturn { Code = PlayReturnCode.TilesDontMakedValidRow };
            return new PlayReturn { Code = PlayReturnCode.Ok, Points = totalPoints };
        }

        private List<TileOnBoard> GetTiles(List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            var tilesOnBoard = new List<TileOnBoard>();
            foreach (var (TileId, X, Y) in tilesTupleToPlay)
            {
                Tile tile = RepositoryAdapter.GetTileById(TileId);
                var coordinates = new CoordinatesInGame(X, Y);
                TileOnBoard tileOnBoard = new TileOnBoard(tile, coordinates);
                tilesOnBoard.Add(tileOnBoard);
            }
            return tilesOnBoard;
        }

        private List<TileOnPlayer> GetPlayerTiles(List<int> tilesIds)
        {
            var tilesOnPlayer = new List<TileOnPlayer>();
            foreach (var tileId in tilesIds)
            {
                TileOnPlayer tile = RepositoryAdapter.GetTileOnPlayerById(tileId);
                tilesOnPlayer.Add(tile);
            }
            return tilesOnPlayer;
        }

        public Player GetPlayer(int playerId) => RepositoryAdapter.GetPlayer(playerId);

        public Game GetGame(int GameId) => RepositoryAdapter.GetGame(GameId);

        private SwapTilesReturn SwapTiles(Player player, List<TileOnPlayer> tilesToSwap)
        {
            List<byte> rackPositions = RackPositions(tilesToSwap);
            SetNextPlayerTurnToPlay(player.Id);
            RepositoryAdapter.TilesFromBagToPlayer(player, rackPositions);
            RepositoryAdapter.TilesFromPlayerToBag(player, tilesToSwap);
            RepositoryAdapter.UpdatePlayer(player);
            return new SwapTilesReturn { Code = PlayReturnCode.Ok, NewRack = GetPlayer(player.Id).Rack };
        }

        private void RefreshPlayer(Player player)
        {
            for (int i = 0; i < Game.Players.Count; i++) //todo
                Game.Players[i] = GetPlayer(Game.Players[i].Id);
        }

        private Rack PlayTiles(Player player, List<TileOnBoard> tilesToPlay, int points)
        {
            player.Points += points;
            player.SetTurn(false);
            Game.Board.Tiles.AddRange(tilesToPlay);
            RepositoryAdapter.UpdatePlayer(player);
            SetNextPlayerTurnToPlay(player.Id);

            // TODO
            #warning todo
            #region todo
            var rackPositions = new List<byte>();
            for (byte i = 0; i < tilesToPlay.Count; i++)
                rackPositions.Add(i);
            #endregion

            RepositoryAdapter.TilesFromBagToPlayer(player, rackPositions);
            RepositoryAdapter.TilesFromPlayerToGame(Game.Id, player.Id, tilesToPlay);
            return RepositoryAdapter.GetPlayer(player.Id).Rack;
        }

        private void SetNextPlayerTurnToPlay(int playerId)
        {
            Player thisPlayer = GetPlayer(playerId); //todo : appel base peut être évité en stockant thisPlayer
            int position = Game.Players.FirstOrDefault(p => p.Id == playerId).GamePosition;
            int playersNumber = Game.Players.Count;
            int nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            Player nexPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            thisPlayer.SetTurn(false);
            nexPlayer.SetTurn(true);
            RepositoryAdapter.UpdatePlayer(thisPlayer);
            RepositoryAdapter.UpdatePlayer(nexPlayer);
        }

        private int CountTilesMakedValidRow(List<TileOnBoard> tiles)
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
                        if ((points = CountTilesMakedValidColumn(new List<TileOnBoard> { tile })) == 0) return 0;
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
                        if ((points = CountTilesMakedValidLine(new List<TileOnBoard> { tile })) == 0) return 0;
                        if (points != 1) tatalPoints += points;
                    }
                }
            }
            return tatalPoints;
        }

        private int CountTilesMakedValidLine(List<TileOnBoard> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.X); var max = tiles.Max(t => t.Coordinates.X);
            var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && min <= t.Coordinates.X && t.Coordinates.X <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesRight = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X >= max).OrderBy(t => t.Coordinates.X).ToList();
            var tilesRightConsecutive = tilesRight.FirstConsecutives(Direction.Right, max);
            allTilesAlongReferenceTiles.AddRange(tilesRightConsecutive);

            var tilesLeft = Game.Board.Tiles.Where(t => t.Coordinates.Y == tiles[0].Coordinates.Y && t.Coordinates.X <= min).OrderByDescending(t => t.Coordinates.X).ToList();
            var tilesLeftConsecutive = tilesLeft.FirstConsecutives(Direction.Left, min);
            allTilesAlongReferenceTiles.AddRange(tilesLeftConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return allTilesAlongReferenceTiles.Count != TILES_NUMBER_FOR_A_QWIRKLE ? allTilesAlongReferenceTiles.Count : POINTS_FOR_A_QWIRKLE;
        }

        private int CountTilesMakedValidColumn(List<TileOnBoard> tiles)
        {
            var allTilesAlongReferenceTiles = tiles.ToList();
            var min = tiles.Min(t => t.Coordinates.Y); var max = tiles.Max(t => t.Coordinates.Y);
            var tilesBetweenReference = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && min <= t.Coordinates.Y && t.Coordinates.Y <= max);
            allTilesAlongReferenceTiles.AddRange(tilesBetweenReference);

            var tilesUp = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y >= max).OrderBy(t => t.Coordinates.Y).ToList();
            var tilesUpConsecutive = tilesUp.FirstConsecutives(Direction.Top, max);
            allTilesAlongReferenceTiles.AddRange(tilesUpConsecutive);

            var tilesBottom = Game.Board.Tiles.Where(t => t.Coordinates.X == tiles[0].Coordinates.X && t.Coordinates.Y <= min).OrderByDescending(t => t.Coordinates.Y).ToList();
            var tilesBottomConsecutive = tilesBottom.FirstConsecutives(Direction.Bottom, min);
            allTilesAlongReferenceTiles.AddRange(tilesBottomConsecutive);

            if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.AreRowByTileRespectsRules())
                return 0;

            return allTilesAlongReferenceTiles.Count != TILES_NUMBER_FOR_A_QWIRKLE ? allTilesAlongReferenceTiles.Count : POINTS_FOR_A_QWIRKLE;
        }

        private static bool AreNumbersConsecutive(List<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

        private void SetPlayerTurn(int playerId, bool turn)
        {
            RepositoryAdapter.SetPlayerTurn(playerId, turn);
            Game.Players.FirstOrDefault(p => p.Id == playerId).SetTurn(turn);
        }

        private static List<byte> RackPositions(List<TileOnPlayer> tiles)
        {
            var rackPositions = new List<byte>();
            foreach (var tileToSwap in tiles)
                rackPositions.Add(tileToSwap.RackPosition);
            return rackPositions;
        }
    }
}
