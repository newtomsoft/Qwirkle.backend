using Qwirkle.Core.Entities;
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

        private readonly IRepository _repositoryAdapter;

        public Game Game { get; set; }

        public CoreUseCase(IRepository repositoryAdapter)
        {
            _repositoryAdapter = repositoryAdapter;
        }

        public List<Player> CreateGame(List<int> usersIds)
        {
            Game = _repositoryAdapter.CreateGame(DateTime.Now);
            CreatePlayers(usersIds);
            CreateTiles();
            DealTilesToPlayers();
            RefreshPlayers();
            SelectFirstPlayer();
            return Game.Players;
        }

        public ArrangeRackReturn TryArrangeRack(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesToArrangeTuple)
        {
            Player player = GetPlayer(playerId);
            var tilesIds = new List<int>();
            foreach (var tiles in GetTiles(tilesToArrangeTuple))
                tilesIds.Add(tiles.Id);

            if (!player.HasTiles(tilesIds)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDontHaveThisTile};

            var tilesToArrange = GetPlayerTiles(tilesIds);
            ArrangeRack(player, tilesToArrange);
            return new ArrangeRackReturn() { Code = PlayReturnCode.Ok};
        }

        private void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange) => _repositoryAdapter.ArrangeRack(player, tilesToArrange);

        public PlayReturn TryPlayTiles(int playerId, List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            Player player = GetPlayer(playerId);
            if (!player.IsTurn) return new PlayReturn { Code = PlayReturnCode.NotPlayerTurn, GameId = player.GameId };

            var tilesToPlay = GetTiles(tilesTupleToPlay);
            var tilesIds = new List<int>();
            foreach (var tiles in tilesToPlay)
                tilesIds.Add(tiles.Id);

            Game = GetGame(player.GameId);
            if (!player.HasTiles(tilesIds)) return new PlayReturn { Code = PlayReturnCode.PlayerDontHaveThisTile, GameId = Game.Id };

            PlayReturn playReturn = GetPlayReturn(tilesToPlay, player);
            if (playReturn.Code != PlayReturnCode.Ok) return playReturn;

            playReturn.NewRack = PlayTiles(player, tilesToPlay, playReturn.Points);
            return playReturn;
        }

        public SwapTilesReturn TrySwapTiles(int playerId, List<int> tilesIds)
        {
            Player player = GetPlayer(playerId);
            Game = GetGame(player.GameId);
            if (!player.IsTurn) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
            if (!player.HasTiles(tilesIds)) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.PlayerDontHaveThisTile };

            List<TileOnPlayer> tilesToSwap = GetPlayerTiles(tilesIds);
            var swapTilesReturn = SwapTiles(player, tilesToSwap);
            return swapTilesReturn;
        }

        public SkipTurnReturn TrySkipTurn(int playerId)
        {
            Player player = GetPlayer(playerId);
            Game = GetGame(player.GameId);
            if (!player.IsTurn) return new SkipTurnReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };

            SkipTurnReturn SkipTurnReturn = SkipTurn(player);
            return SkipTurnReturn;
        }

        private void DealTilesToPlayers()
        {
            var rackPositions = new List<byte>();
            for (byte i = 0; i < TILES_NUMBER_PER_PLAYER; i++)
                rackPositions.Add(i);

            foreach (var player in Game.Players)
                _repositoryAdapter.TilesFromBagToPlayer(player, rackPositions);
        }

        private void CreateTiles() => _repositoryAdapter.CreateTiles(Game.Id);

        private void CreatePlayers(List<int> usersIds)
        {
            Game.Players = new List<Player>();
            usersIds.ForEach(userId => Game.Players.Add(_repositoryAdapter.CreatePlayer(userId, Game.Id)));
            SetPositionsPlayers();
            Game.Players.ForEach(player => _repositoryAdapter.UpdatePlayer(player));
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
            SetPlayerTurn(playerIdToPlay);
        }

        public PlayReturn GetPlayReturn(List<TileOnBoard> tilesPlayed, Player player)
        {
            if (Game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn { Code = PlayReturnCode.Ok, Points = 1, TilesPlayed = tilesPlayed, GameId = Game.Id, };

            bool allTilesIsolated = true;
            foreach (var tile in tilesPlayed)
                if (Game.Board.IsIsolatedTile(tile))
                    allTilesIsolated = false;
            if (Game.Board.Tiles.Count > 0 && allTilesIsolated) return new PlayReturn { Code = PlayReturnCode.TileIsolated, Points = 0, GameId = Game.Id };

            int wonPoints = CountTilesMakedValidRow(tilesPlayed);
            if (wonPoints == 0) return new PlayReturn { Code = PlayReturnCode.TilesDontMakedValidRow, GameId = Game.Id };

            if (Game.Bag.Tiles.Count == 0 && tilesPlayed.Count == player.Rack.Tiles.Count)
            {
                var pointsWonWhenPlayerFinishTheGame = 6;
                wonPoints += pointsWonWhenPlayerFinishTheGame;
                _repositoryAdapter.SetGameOver(Game.Id);
            }

            return new PlayReturn { Code = PlayReturnCode.Ok, Points = wonPoints, GameId = Game.Id, TilesPlayed = tilesPlayed };
        }

        private List<TileOnBoard> GetTiles(List<(int tileId, sbyte x, sbyte y)> tilesTupleToPlay)
        {
            var tilesOnBoard = new List<TileOnBoard>();
            foreach (var (TileId, X, Y) in tilesTupleToPlay)
            {
                Tile tile = _repositoryAdapter.GetTileById(TileId);
                var coordinates = new CoordinatesInGame(X, Y);
                var tileOnBoard = new TileOnBoard(tile, coordinates);
                tilesOnBoard.Add(tileOnBoard);
            }
            return tilesOnBoard;
        }

        private List<TileOnPlayer> GetPlayerTiles(List<int> tilesIds)
        {
            var tilesOnPlayer = new List<TileOnPlayer>();
            foreach (var tileId in tilesIds)
            {
                TileOnPlayer tile = _repositoryAdapter.GetTileOnPlayerById(tileId);
                tilesOnPlayer.Add(tile);
            }
            return tilesOnPlayer;
        }

        public Player GetPlayer(int playerId) => _repositoryAdapter.GetPlayer(playerId);
        public string GetPlayerNameTurn(int gameId) => _repositoryAdapter.GetPlayerNameTurn(gameId);
        public int GetPlayerIdToPlay(int gameId) => _repositoryAdapter.GetPlayerIdToPlay(gameId);
        public List<int> GetListGameIDWithPlayer() => _repositoryAdapter.GetListGameIDWithPlayer();

        public List<string> GetListNamePlayer(int gameId) => _repositoryAdapter.GetListNamePlayer(gameId);
        public Game GetGame(int GameId) => _repositoryAdapter.GetGame(GameId);

        public List<int> GetWinnersPlayersId(int gameId)
        {
            if (!_repositoryAdapter.IsGameOver(gameId))
                return null;

            return _repositoryAdapter.GetLeadersPlayersId(gameId);
        }

        private SkipTurnReturn SkipTurn(Player player)
        {
            player.LastTurnSkipped = true;
            if (Game.Bag.Tiles.Count == 0 && Game.Players.Count(p => p.LastTurnSkipped) == Game.Players.Count)
            {
                _repositoryAdapter.UpdatePlayer(player);
                _repositoryAdapter.SetGameOver(player.GameId);
            }
            else
                SetNextPlayerTurnToPlay(player);

            return new SkipTurnReturn { GameId = player.GameId, Code = PlayReturnCode.Ok };
        }

        private SwapTilesReturn SwapTiles(Player player, List<TileOnPlayer> tilesToSwap)
        {
            var positionsInRack = PositionsInRack(tilesToSwap);
            SetNextPlayerTurnToPlay(player);
            _repositoryAdapter.TilesFromBagToPlayer(player, positionsInRack);
            _repositoryAdapter.TilesFromPlayerToBag(player, tilesToSwap);
            _repositoryAdapter.UpdatePlayer(player);
            return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = GetPlayer(player.Id).Rack };
        }

        private Rack PlayTiles(Player player, List<TileOnBoard> tilesToPlay, int points)
        {
            player.Points += points;
            Game.Board.Tiles.AddRange(tilesToPlay);
            SetNextPlayerTurnToPlay(player);

            // TODO
#warning todo
            #region todo
            var rackPositions = new List<byte>();
            for (byte i = 0; i < tilesToPlay.Count; i++)
                rackPositions.Add(i);
            #endregion

            _repositoryAdapter.TilesFromBagToPlayer(player, rackPositions);
            _repositoryAdapter.TilesFromPlayerToGame(Game.Id, player.Id, tilesToPlay);
            return _repositoryAdapter.GetPlayer(player.Id).Rack;
        }

        private void SetNextPlayerTurnToPlay(Player player)
        {
            if (Game.GameOver) return;

            if (Game.Players.Count == 1)
            {
                player.SetTurn(true);
                _repositoryAdapter.UpdatePlayer(player);
            }
            else
            {
                int position = Game.Players.FirstOrDefault(p => p.Id == player.Id).GamePosition;
                int playersNumber = Game.Players.Count;
                int nextPlayerPosition = position < playersNumber ? position + 1 : 1;
                Player nextPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
                player.SetTurn(false);
                nextPlayer.SetTurn(true);
                _repositoryAdapter.UpdatePlayer(player);
                _repositoryAdapter.UpdatePlayer(nextPlayer);
            }
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

        private void SetPlayerTurn(int playerId)
        {
            _repositoryAdapter.SetPlayerTurn(playerId);
            Game.Players.FirstOrDefault(p => p.Id == playerId).SetTurn(true);
        }

        private static List<byte> PositionsInRack(List<TileOnPlayer> tiles)
        {
            var positionsInRack = new List<byte>();
            foreach (var tile in tiles)
                positionsInRack.Add(tile.RackPosition);
            return positionsInRack;
        }
    }
}
