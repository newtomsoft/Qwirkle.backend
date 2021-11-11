﻿namespace Qwirkle.Core.UseCases;

public class CoreUseCase
{
    private const int TilesNumberPerPlayer = 6;
    private const int TilesNumberForAQwirkle = 6;
    private const int PointsForAQwirkle = 12;

    private readonly IRepository _repository;

    public Game Game { get; set; }

    public CoreUseCase(IRepository repository)
    {
        _repository = repository;
    }

    public List<Player> CreateGame(List<int> usersIds)
    {
        Game = _repository.CreateGame(DateTime.UtcNow);
        CreatePlayers(usersIds);
        CreateTiles();
        DealTilesToPlayers();
        SelectFirstPlayer();
        return Game.Players;
    }

    public ArrangeRackReturn TryArrangeRack(int playerId, List<(int tileId, Abscissa x, Ordinate y)> tilesToArrangeTuple)
    {
        Player player = GetPlayer(playerId);
        var tilesIds = new List<int>();
        foreach (var tiles in GetTiles(tilesToArrangeTuple))
            tilesIds.Add(tiles.Id);

        if (!player.HasTiles(tilesIds)) return new ArrangeRackReturn { Code = PlayReturnCode.PlayerDontHaveThisTile };

        var tilesToArrange = GetPlayerTiles(playerId, tilesIds);
        ArrangeRack(player, tilesToArrange);
        return new ArrangeRackReturn() { Code = PlayReturnCode.Ok };
    }

    public PlayReturn TryPlayTiles(int playerId, List<(int tileId, Abscissa x, Ordinate y)> tilesTupleToPlay)
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

    public PlayReturn TryPlayTilesSimulation(int playerId, List<(int tileId, Abscissa x, Ordinate y)> tilesTupleToPlay)
    {
        var player = GetPlayer(playerId);
        var tilesToPlay = GetTiles(tilesTupleToPlay);
        Game = GetGame(player.GameId);
        return GetPlayReturn(tilesToPlay, player, true);
    }

    public SwapTilesReturn TrySwapTiles(int playerId, List<int> tilesIds)
    {
        Player player = GetPlayer(playerId);
        Game = GetGame(player.GameId);
        if (!player.IsTurn) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
        if (!player.HasTiles(tilesIds)) return new SwapTilesReturn { GameId = Game.Id, Code = PlayReturnCode.PlayerDontHaveThisTile };

        List<TileOnPlayer> tilesToSwap = GetPlayerTiles(playerId, tilesIds);
        var swapTilesReturn = SwapTiles(player, tilesToSwap);
        return swapTilesReturn;
    }

    public SkipTurnReturn TrySkipTurn(int playerId)
    {
        var player = GetPlayer(playerId);
        Game = GetGame(player.GameId);
        return player.IsTurn ? SkipTurn(player) : new SkipTurnReturn { GameId = Game.Id, Code = PlayReturnCode.NotPlayerTurn };
    }

    private void ArrangeRack(Player player, List<TileOnPlayer> tilesToArrange) => _repository.ArrangeRack(player, tilesToArrange);

    private void DealTilesToPlayers()
    {
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberPerPlayer; i++)
            rackPositions.Add(i);

        foreach (var player in Game.Players)
            _repository.TilesFromBagToPlayer(player, rackPositions);
    }

    private void CreateTiles() => _repository.CreateTiles(Game.Id);

    private void CreatePlayers(List<int> usersIds)
    {
        Game.Players = new List<Player>();
        usersIds.ForEach(userId => Game.Players.Add(_repository.CreatePlayer(userId, Game.Id)));
        SetPositionsPlayers();
        Game.Players.ForEach(player => _repository.UpdatePlayer(player));
    }

    private void SetPositionsPlayers()
    {
        Game.Players = Game.Players.OrderBy(_ => Guid.NewGuid()).ToList();
        for (int i = 0; i < Game.Players.Count; i++)
            Game.Players[i].GamePosition = (byte)(i + 1);
    }

    private void SelectFirstPlayer()
    {
        var playersWithNumberCanBePlayedTiles = new Dictionary<int, int>();
        Game.Players.ForEach(p => playersWithNumberCanBePlayedTiles[p.Id] = p.TilesNumberCanBePlayedAtGameBeginning());
        var playerIdToPlay = playersWithNumberCanBePlayedTiles.OrderByDescending(p => p.Value).ThenBy(_ => Guid.NewGuid()).Select(p => p.Key).First();
        SetPlayerTurn(playerIdToPlay);
    }

    public PlayReturn GetPlayReturn(List<TileOnBoard> tilesPlayed, Player player, bool simulationMode = false)
    {
        if (Game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn { Code = PlayReturnCode.Ok, Points = 1, TilesPlayed = tilesPlayed, GameId = Game.Id, };

        bool allTilesIsolated = true;
        foreach (var tile in tilesPlayed)
            if (Game.Board.IsIsolatedTile(tile))
                allTilesIsolated = false;
        if (Game.Board.Tiles.Count > 0 && allTilesIsolated) return new PlayReturn { Code = PlayReturnCode.TileIsolated, Points = 0, GameId = Game.Id };

        int wonPoints = CountTilesMakeValidRow(tilesPlayed);
        if (wonPoints == 0) return new PlayReturn { Code = PlayReturnCode.TilesDontMakedValidRow, GameId = Game.Id };

        if (Game.Bag?.Tiles.Count == 0 && tilesPlayed.Count == player.Rack.Tiles.Count)
        {
            var pointsWonWhenPlayerFinishTheGame = 6;
            wonPoints += pointsWonWhenPlayerFinishTheGame;
            if (!simulationMode) _repository.SetGameOver(Game.Id);
        }

        return new PlayReturn { Code = PlayReturnCode.Ok, Points = wonPoints, GameId = Game.Id, TilesPlayed = tilesPlayed };
    }

    private List<TileOnBoard> GetTiles(List<(int tileId, Abscissa x, Ordinate y)> tilesTupleToPlay)
    {
        var tilesOnBoard = new List<TileOnBoard>();
        foreach (var (tileId, x, y) in tilesTupleToPlay)
        {
            var tile = _repository.GetTileById(tileId);
            var coordinates = new CoordinatesInGame(x, y);
            var tileOnBoard = new TileOnBoard(tile, coordinates);
            tilesOnBoard.Add(tileOnBoard);
        }
        return tilesOnBoard;
    }

    private List<TileOnPlayer> GetPlayerTiles(int playerId, List<int> tilesIds)
    {
        var tilesOnPlayer = new List<TileOnPlayer>();
        foreach (var tileId in tilesIds)
        {
            TileOnPlayer tile = _repository.GetTileOnPlayerById(playerId, tileId);
            tilesOnPlayer.Add(tile);
        }
        return tilesOnPlayer;
    }

    public Player GetPlayer(int playerId) => _repository.GetPlayer(playerId);
    public Player GetPlayer(int gameId, int userId) => _repository.GetPlayer(gameId, userId);

    public string GetPlayerNameTurn(int gameId) => _repository.GetPlayerNameTurn(gameId);
    public int GetPlayerIdToPlay(int gameId) => _repository.GetPlayerIdToPlay(gameId);
    public List<int> GetGamesIdsContainingPlayers() => _repository.GetGamesIdsContainingPlayers();
    public List<int> GetUsersId() => _repository.GetUsersId();
    public List<int> GetUserGames(int userId) => _repository.GetUserGames(userId);
    public Game GetGame(int gameId) => _repository.GetGame(gameId);

    public List<int> GetWinnersPlayersId(int gameId)
    {
        if (!_repository.IsGameOver(gameId))
            return null;

        return _repository.GetLeadersPlayersId(gameId);
    }

    private SkipTurnReturn SkipTurn(Player player)
    {
        player.LastTurnSkipped = true;
        if (Game.Bag.Tiles.Count == 0 && Game.Players.Count(p => p.LastTurnSkipped) == Game.Players.Count)
        {
            _repository.UpdatePlayer(player);
            _repository.SetGameOver(player.GameId);
        }
        else
            SetNextPlayerTurnToPlay(player);

        return new SkipTurnReturn { GameId = player.GameId, Code = PlayReturnCode.Ok };
    }

    private SwapTilesReturn SwapTiles(Player player, List<TileOnPlayer> tilesToSwap)
    {
        var positionsInRack = PositionsInRack(tilesToSwap);
        SetNextPlayerTurnToPlay(player);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBag(player, tilesToSwap);
        _repository.UpdatePlayer(player);
        return new SwapTilesReturn { GameId = player.GameId, Code = PlayReturnCode.Ok, NewRack = GetPlayer(player.Id).Rack };
    }

    private Rack PlayTiles(Player player, List<TileOnBoard> tilesToPlay, int points)
    {
        player.LastTurnPoints = points;
        player.Points += points;
        Game.Board.Tiles.AddRange(tilesToPlay);
        SetNextPlayerTurnToPlay(player);

        var positionsInRack = new List<byte>();
        for (byte i = 0; i < tilesToPlay.Count; i++) positionsInRack.Add(i);
        _repository.TilesFromBagToPlayer(player, positionsInRack);
        _repository.TilesFromPlayerToBoard(Game.Id, player.Id, tilesToPlay);
        return _repository.GetPlayer(player.Id).Rack;
    }

    private void SetNextPlayerTurnToPlay(Player player)
    {
        if (Game.GameOver) return;

        if (Game.Players.Count == 1)
        {
            player.SetTurn(true);
            _repository.UpdatePlayer(player);
        }
        else
        {
            var position = Game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = Game.Players.Count;
            var nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            var nextPlayer = Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            nextPlayer!.SetTurn(true);
            _repository.UpdatePlayer(player);
            _repository.UpdatePlayer(nextPlayer);
        }
    }

    private int CountTilesMakeValidRow(List<TileOnBoard> tiles)
    {
        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) != tiles.Count && tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count)
            return 0;

        var totalPoints = 0;
        int points;
        if (tiles.Count(t => t.Coordinates.Y == tiles[0].Coordinates.Y) == tiles.Count)
        {
            if ((points = CountTilesMakeValidLine(tiles)) == 0) return 0;
            if (points != 1) totalPoints += points;
            if (tiles.Count > 1)
            {
                foreach (var tile in tiles)
                {
                    if ((points = CountTilesMakeValidColumn(new List<TileOnBoard> { tile })) == 0) return 0;
                    if (points != 1) totalPoints += points;
                }
            }
        }

        if (tiles.Count(t => t.Coordinates.X == tiles[0].Coordinates.X) != tiles.Count) return totalPoints;
        if ((points = CountTilesMakeValidColumn(tiles)) == 0) return 0;
        if (points != 1) totalPoints += points;
        if (tiles.Count <= 1) return totalPoints;
        foreach (var tile in tiles)
        {
            if ((points = CountTilesMakeValidLine(new List<TileOnBoard> { tile })) == 0) return 0;
            if (points != 1) totalPoints += points;
        }
        return totalPoints;
    }

    private int CountTilesMakeValidLine(IReadOnlyList<TileOnBoard> tiles)
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

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.X).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private int CountTilesMakeValidColumn(IReadOnlyList<TileOnBoard> tiles)
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

        if (!AreNumbersConsecutive(allTilesAlongReferenceTiles.Select(t => t.Coordinates.Y).ToList()) || !allTilesAlongReferenceTiles.FormCompliantRow())
            return 0;

        return allTilesAlongReferenceTiles.Count != TilesNumberForAQwirkle ? allTilesAlongReferenceTiles.Count : PointsForAQwirkle;
    }

    private static bool AreNumbersConsecutive(IReadOnlyCollection<sbyte> numbers) => numbers.Count > 0 && numbers.Distinct().Count() == numbers.Count && numbers.Min() + numbers.Count - 1 == numbers.Max();

    private void SetPlayerTurn(int playerId)
    {
        _repository.SetPlayerTurn(playerId);
        Game.Players.First(p => p.Id == playerId).SetTurn(true);
    }

    private static List<byte> PositionsInRack(IEnumerable<TileOnPlayer> tiles) => tiles.Select(tile => tile.RackPosition).ToList();
}
