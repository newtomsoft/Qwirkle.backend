using Qwirkle.Infra.Repository.DaoExtensionMethods;

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
        var useCase = new CoreUseCase(repository, null, infoUseCase, null);
        var usersIds = infoUseCase.GetAllUsersId();
        var players = useCase.CreateGame(usersIds.ToHashSet()).OrderBy(p => p.Id).ToList();
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
    public void Return6NoComboTilesWhenBoardIsEmptyAndNoCombinationPossibleInRack()
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

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
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
        combo3Tiles.Count.ShouldBe(6 * 5 * 4 * 2); // 6 first tile x 5 second tile x 4 third tile x 2 positions for last

        var combo4Tiles = TilesCombination(4, playReturns);
        combo4Tiles.Count.ShouldBe(1440);

        var combo5Tiles = TilesCombination(5, playReturns);
        combo5Tiles.Count.ShouldBe(5760);

        var combo6Tiles = TilesCombination(6, playReturns);
        combo6Tiles.Count.ShouldBe(11520);

        //todo algo must return combos without coordinates changing :
        //combo3Tiles.Count.ShouldBe(6 * 5 * 4);
        //combo4Tiles.Count.ShouldBe(6 * 5 * 4 * 3);
        //combo5Tiles.Count.ShouldBe(6 * 5 * 4 * 3 * 2);
        //combo6Tiles.Count.ShouldBe(6 * 5 * 4 * 3 * 2 * 1);
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
        combo3Tiles.Count.ShouldBe(3 * 2 * 1 * 2); // 3 first tile x 2 second tile x 1 third tile * 2 positions for last

        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
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
        combo3Tiles.Count.ShouldBe(3 * 2 * 1 * 2); // 3 first tile x 2 second tile x 1 third tile * 2 positions for last tile

        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void Return4NoComboTilesAndWhenBoardIsEmptyAndNoCombinationPossibleInRackAnd2X2DuplicatesTiles()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Orange && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Orange && t.Shape == TileShape.Diamond);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.FourPointStar);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Square);
        var expectedTilesPlayable = new List<TileDao> { constTile0!, constTile2!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botUseCase.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(4); // 4 tiles from the rack are doable because 2 other are sames
        noComboTile.Select(t => t[0].Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t[0].ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape).SequenceEqual(expectedTilesPlayable.Select(t => t.ToTile()).OrderBy(t => t.Color).ThenBy(t => t.Shape)).ShouldBeTrue();

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }


    //Todo test with board not empty
}