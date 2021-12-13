namespace Qwirkle.Test;

public class GetDoableMovesShould
{
    #region constructor and privates
    public GetDoableMovesShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        connectionFactory.Add4DefaultTestUsers();
        var repository = new Repository(_dbContext);
        var infoUseCase = new InfoUseCase(repository, null);
        var useCase = new CoreUseCase(repository, null, infoUseCase);
        var usersIds = infoUseCase.GetAllUsersId();
        var players = useCase.CreateGame(usersIds).OrderBy(p => p.Id).ToList();
        _botUseCase = new BotUseCase(infoUseCase, useCase);
        _player = players[0];
        _userId = usersIds[0];
    }

    private readonly DefaultDbContext _dbContext;
    private readonly BotUseCase _botUseCase;
    private readonly Player _player;
    private readonly int _userId;

    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < 6; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        _dbContext.SaveChanges();
    }

    private static List<List<TileOnBoard>> TilesCombination(int tilesNumberInCombo, IEnumerable<PlayReturn> playReturns) => playReturns.Where(p => p.TilesPlayed.Count == tilesNumberInCombo).Select(p => p.TilesPlayed).ToList();
    #endregion

    [Fact]
    public void Return6NoComboTilesAnd0Combo2TilesWhenBoardIsEmptyAndNoCombinationPossibleInRack()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Blue && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Orange && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botUseCase.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t[0].Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t[0].ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var combo2Tiles = TilesCombination(2, playReturns);
        combo2Tiles.Count.ShouldBe(0); // no combination possible

        var combo3Tiles = TilesCombination(3, playReturns);
        combo3Tiles.Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void ReturnMaxItemsWhenBoardIsEmptyAndMaxCombinationInRackIsPossible()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botUseCase.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t[0].Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t[0].ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var combo2Tiles = TilesCombination(2, playReturns);
        combo2Tiles.Count.ShouldBe(6 * 5); // 6 first tile x 5 second tile

        var combo3Tiles = TilesCombination(3, playReturns);
        combo3Tiles.Count.ShouldBe(6 * 5 * 4); // 6 first tile x 5 second tile x 4 third tile
    }

    [Fact]
    public void ReturnOtherItemsWhenBoardIsEmptyAndOtherRack()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.EightPointStar);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botUseCase.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t[0].Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t[0].ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack
                                                                                                                                                                                                      // 
        var combo2Tiles = TilesCombination(2, playReturns);
        combo2Tiles.Count.ShouldBe(3 * 2); // 3 first tile x 2 second tile

        var combo3Tiles = TilesCombination(3, playReturns);
        combo3Tiles.Count.ShouldBe(3 * 2 * 1); // 3 first tile x 2 second tile x 1 third tile
    }

    [Fact]
    public void Return3Combos2GreenTiles()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.Circle);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.Diamond);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botUseCase.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t[0].Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t[0].ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape).SequenceEqual(constTiles.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();// each of tile from rack

        var combo2Tiles = TilesCombination(2, playReturns);
        combo2Tiles.Count.ShouldBe((3 + 1 + 1) * 2); // (3 combo greens + 1 combo circle + 1 combo diamond) *2

        var combo3Tiles = TilesCombination(3, playReturns);
        combo3Tiles.Count.ShouldBe(3 * 2 * 1); // 3 first tile x 2 second tile x 1 third tile
    }
}