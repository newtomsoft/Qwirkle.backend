namespace Qwirkle.Test;

public class GetDoableMovesShould
{
    private readonly DefaultDbContext _dbContext;
    private readonly InfoUseCase _infoUseCase;
    private readonly CoreUseCase _useCase;
    private readonly BotUseCase _botUseCase;

    public GetDoableMovesShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        connectionFactory.Add4DefaultTestUsers();

        var repository = new Repository(_dbContext);
        _infoUseCase = new InfoUseCase(repository, null);
        _useCase = new CoreUseCase(repository, null, _infoUseCase);
        _botUseCase = new BotUseCase(_infoUseCase, _useCase);
    }

    #region private methods
    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void Return6ItemsWhenBoardIsEmptyAndNoCombinationPossibleInRack()
    {
        var usersIds = _infoUseCase.GetAllUsersId();
        var players = _useCase.CreateGame(usersIds);
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Blue && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Orange && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(players[0].Id, constTiles);

        var gameId = players[0].GameId;
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, usersIds[0]);
        playReturns.Count.ShouldBe(6); // 6 tiles from the rack are all doable

        var playReturnsWith1Tile = playReturns.Where(p => p.TilesPlayed.Count == 1).ToList();
        var tilesPlayedSingle = playReturnsWith1Tile.Select(p => p.TilesPlayed[0]).OrderBy(t => t.Color).ThenBy(t => t.Shape).ToList();
        playReturnsWith1Tile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        tilesPlayedSingle.Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        tilesPlayedSingle.Select(t => t.ToTile()).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var playReturnsWith2Tiles = playReturns.Where(p => p.TilesPlayed.Count == 2).ToList();
        playReturnsWith2Tiles.Count.ShouldBe(0); // no combination possible with these 6 tiles
    }

    [Fact]
    public void ReturnMaxItemsWhenBoardIsEmptyAndMaxCombinationInRackIsPossible()
    {
        var usersIds = _infoUseCase.GetAllUsersId();
        var players = _useCase.CreateGame(usersIds);
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(players[0].Id, constTiles);

        var gameId = players[0].GameId;
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, usersIds[0]);

        var playReturnsWith1Tile = playReturns.Where(p => p.TilesPlayed.Count == 1).ToList();
        var tilesPlayedSingle = playReturnsWith1Tile.Select(p => p.TilesPlayed[0]).OrderBy(t => t.Color).ThenBy(t => t.Shape).ToList();

        playReturnsWith1Tile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        tilesPlayedSingle.Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        tilesPlayedSingle.Select(t => t.ToTile()).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var playReturnsWith2Tiles = playReturns.Where(p => p.TilesPlayed.Count == 2).ToList();
        var tilesPlayedWith2Tiles = playReturnsWith2Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith2Tiles.Count.ShouldBe(6 * 5); // 6 first tile x 5 second tile

        var playReturnsWith3Tiles = playReturns.Where(p => p.TilesPlayed.Count == 3).ToList();
        var tilesPlayedWith3Tiles = playReturnsWith3Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith3Tiles.Count.ShouldBe(6 * 5 * 4); // 6 first tile x 5 second tile x 4 third tile
    }

    [Fact]
    public void TORENAME()
    {
        var usersIds = _infoUseCase.GetAllUsersId();
        var players = _useCase.CreateGame(usersIds);
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(players[0].Id, constTiles);

        var gameId = players[0].GameId;
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, usersIds[0]);

        var playReturnsWith1Tile = playReturns.Where(p => p.TilesPlayed.Count == 1).ToList();
        var tilesPlayedSingle = playReturnsWith1Tile.Select(p => p.TilesPlayed[0]).OrderBy(t => t.Color).ThenBy(t => t.Shape).ToList();

        playReturnsWith1Tile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        tilesPlayedSingle.Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        tilesPlayedSingle.Select(t => t.ToTile()).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var playReturnsWith2Tiles = playReturns.Where(p => p.TilesPlayed.Count == 2).ToList();
        var tilesPlayedWith2Tiles = playReturnsWith2Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith2Tiles.Count.ShouldBe(3 * 2); // 3 first tile x 2 second tile

        var playReturnsWith3Tiles = playReturns.Where(p => p.TilesPlayed.Count == 3).ToList();
        var tilesPlayedWith3Tiles = playReturnsWith3Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith3Tiles.Count.ShouldBe(3 * 2 * 1); // 3 first tile x 2 second tile x 1 third tile
    }

    [Fact]
    public void TORENAME2()
    {
        var usersIds = _infoUseCase.GetAllUsersId();
        var players = _useCase.CreateGame(usersIds);
        players = players.OrderBy(p => p.Id).ToList();
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.Circle);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.Diamond);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(players[0].Id, constTiles);

        var gameId = players[0].GameId;
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, usersIds[0]);

        var playReturnsWith1Tile = playReturns.Where(p => p.TilesPlayed.Count == 1).ToList();
        var tilesPlayedSingle = playReturnsWith1Tile.Select(p => p.TilesPlayed[0]).OrderBy(t => t.Color).ThenBy(t => t.Shape).ToList();

        playReturnsWith1Tile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        tilesPlayedSingle.Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        tilesPlayedSingle.Select(t => t.ToTile()).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var playReturnsWith2Tiles = playReturns.Where(p => p.TilesPlayed.Count == 2).ToList();
        var tilesPlayedWith2Tiles = playReturnsWith2Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith2Tiles.Count.ShouldBe(10);

        var playReturnsWith3Tiles = playReturns.Where(p => p.TilesPlayed.Count == 3).ToList();
        var tilesPlayedWith3Tiles = playReturnsWith3Tiles.Select(p => p.TilesPlayed).ToList();
        tilesPlayedWith3Tiles.Count.ShouldBe(3 * 2 * 1); // 3 first tile x 2 second tile x 1 third tile
    }
}