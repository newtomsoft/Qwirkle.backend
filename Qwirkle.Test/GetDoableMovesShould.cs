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
        var infoUseCase = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        var useCase = new CoreService(repository, null, infoUseCase, new Logger<CoreService>(new LoggerFactory()));
        var usersIds = infoUseCase.GetAllUsersId();
        var players = useCase.CreateGame(usersIds.ToHashSet()).Players.OrderBy(p => p.Id).ToList();
        _botService = new BotService(infoUseCase, useCase, new Logger<CoreService>(new LoggerFactory()));
        _player = players[0];
        _userId = usersIds[0];
    }

    private readonly DefaultDbContext _dbContext;
    private readonly BotService _botService;
    private readonly Player _player;
    private readonly int _userId;

    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < newTiles.Count; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        for (var i = newTiles.Count; i < CoreService.TilesNumberPerPlayer; i++) _dbContext.TilesOnPlayer.Remove(tilesOnPlayer[i]);
        _dbContext.SaveChanges();
    }

    private static HashSet<HashSet<TileOnBoard>> TilesCombination(int tilesNumberInCombo, IEnumerable<PlayReturn> playReturns) => playReturns.Where(p => p.Move.Tiles.Count == tilesNumberInCombo).Select(p => p.Move.Tiles.ToHashSet()).ToHashSet();
    private int TileId(TileShape shape, TileColor color, int idIndex = 0) => Tile(shape, color, idIndex).Id;
    private TileDao Tile(TileShape shape, TileColor color, int idIndex = 0) => _dbContext.Tiles.Where(t => t.Shape == shape && t.Color == color).OrderBy(t => t.Id).AsEnumerable().ElementAt(idIndex);

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

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void Return1WhenBoardIsEmptyAnd1TileInRack()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTiles = new List<TileDao> { constTile0! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(1); // just 1 tile from the rack is doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

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

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

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
    public void Return2CombosWhenBoardIsEmptyAndOnly2TilesInRackWitchCanMakeRow()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTiles = new List<TileDao> { constTile0!, constTile1! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(2); // 2 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

        TilesCombination(2, playReturns).Count.ShouldBe(2); // 2 first tile x 1 second tiles
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
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

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack
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
    public void Return3Combos3GreenTiles()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Yellow && t.Shape == TileShape.Clover);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile3 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Purple && t.Shape == TileShape.Circle);
        var constTile4 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Red && t.Shape == TileShape.Diamond);
        var constTile5 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2!, constTile3!, constTile4!, constTile5! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(6); // 6 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

        var combo2Tiles = TilesCombination(2, playReturns);
        combo2Tiles.Count.ShouldBe((3 + 1 + 1) * 2); // (3 combo greens + 1 combo circle + 1 combo diamond) *2

        var combo3Tiles = TilesCombination(3, playReturns);
        combo3Tiles.Count.ShouldBe(3 * 2 * 1 * 2); // 3 first tile x 2 second tile x 1 third tile * 2 positions for last tile

        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void Return3CombosWhenBoardIsEmptyAndOnly3TileInRackWitchCanMakeRow()
    {
        var constTile0 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Circle);
        var constTile1 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Diamond);
        var constTile2 = _dbContext.Tiles.FirstOrDefault(t => t.Color == TileColor.Green && t.Shape == TileShape.Square);
        var constTiles = new List<TileDao> { constTile0!, constTile1!, constTile2! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, constTiles);

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(3); // 3 tiles from the rack are all doable
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t));// each of tile from rack

        TilesCombination(2, playReturns).Count.ShouldBe(6); // 3 first tile x 2 second tiles
        TilesCombination(3, playReturns).Count.ShouldBe(12); // 3 first tile x 2 second tile x 1 third tile * 2 positions for last
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

        var playReturns = _botService.ComputeDoableMoves(_player.GameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(4); // 4 tiles from the rack are doable because 2 other are sames
        noComboTile.Select(t => t.First().Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(expectedTilesPlayable.Select(t => t.ToTile()).OrderBy(t => t));

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void BoardNotEmptyScenario01()
    {
        var gameId = _player.GameId;
        var constTile0 = Tile(TileShape.Circle, TileColor.Orange);
        var expectedTilePlayable = constTile0;
        var playerTiles = new List<TileDao> { constTile0! }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, playerTiles);

        //board construction
        var tilesOnBoard = new List<TileOnBoardDao>
        {
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Blue), PositionX = 10, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Red), PositionX = 11, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow), PositionX = 12, PositionY = 50}
        };
        _dbContext.TilesOnBoard.AddRange(tilesOnBoard);
        _dbContext.SaveChanges();

        var expectedCoordinates = new List<Coordinates>
        {
            Coordinates.From(10, 51), Coordinates.From(11, 51), Coordinates.From(12, 51),
            Coordinates.From(9, 50), Coordinates.From(13, 50),
            Coordinates.From(10, 49), Coordinates.From(11, 49), Coordinates.From(12, 49),
        };

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);

        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(8);
        noComboTile.Select(t => t.First().Coordinates).OrderBy(c => c).ShouldBe(expectedCoordinates.OrderBy(c => c));
        noComboTile.Select(t => t.First().ToTile()).ShouldAllBe(t => t == expectedTilePlayable.ToTile());
        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact]
    public void BoardNotEmptyScenario02()
    {
        var gameId = _player.GameId;
        var constTile0 = Tile(TileShape.Square, TileColor.Orange);
        var constTile1 = Tile(TileShape.Square, TileColor.Blue);
        var constTile2 = Tile(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = Tile(TileShape.FourPointStar, TileColor.Red);
        var constTile4 = Tile(TileShape.Circle, TileColor.Yellow);
        var constTile5 = Tile(TileShape.Diamond, TileColor.Purple);

        var playerTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, playerTiles);

        //board construction
        var tilesOnBoard = new List<TileOnBoardDao>
        {
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Green), PositionX = 10, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow), PositionX = 11, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow, 1), PositionX = 10, PositionY = 51},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Red), PositionX = 11, PositionY = 51},
        };
        _dbContext.TilesOnBoard.AddRange(tilesOnBoard);
        _dbContext.SaveChanges();

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);
        playReturns.Count.ShouldBe(0);
    }

    [Fact]
    public void BoardNotEmptyScenario03()
    {
        var gameId = _player.GameId;
        var constTile0 = Tile(TileShape.Square, TileColor.Orange);
        var constTile1 = Tile(TileShape.Square, TileColor.Blue);
        var constTile2 = Tile(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = Tile(TileShape.Circle, TileColor.Red);
        var constTile4 = Tile(TileShape.Circle, TileColor.Yellow);
        var constTile5 = Tile(TileShape.Diamond, TileColor.Purple);

        var playerTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, playerTiles);

        //board construction
        var tilesOnBoard = new List<TileOnBoardDao>
        {
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Green), PositionX = 10, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow), PositionX = 11, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow, 1), PositionX = 10, PositionY = 51},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Red), PositionX = 11, PositionY = 51},
        };
        _dbContext.TilesOnBoard.AddRange(tilesOnBoard);
        _dbContext.SaveChanges();

        var expectedCoordinatesForNoComboTile = new List<Coordinates>
        {
            Coordinates.From(9, 50), Coordinates.From(10, 49), Coordinates.From(10, 52), Coordinates.From(12, 50),
        };

        var expectedFirstCoordinatesForTwoTilesCombination = new List<Coordinates>
        {
            Coordinates.From(09, 49), Coordinates.From(09, 49), Coordinates.From(09, 52), Coordinates.From(12, 49),
        };

        var expectedSecondCoordinatesForTwoTilesCombination = new List<Coordinates>
        {
            Coordinates.From(09, 50), Coordinates.From(10, 49), Coordinates.From(10, 52), Coordinates.From(12, 50),
        };

        var expectedCoordinatesForTwoTilesCombination = new List<List<Coordinates>>
        {
            new() {Coordinates.From(09, 49), Coordinates.From(09, 50)},
            new() {Coordinates.From(09, 49), Coordinates.From(10, 49)},
            new() {Coordinates.From(09, 52), Coordinates.From(10, 50)},
            new() {Coordinates.From(12, 49), Coordinates.From(12, 52)},
        };

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);
        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(expectedCoordinatesForNoComboTile.Count);
        noComboTile.Select(t => t.First().Coordinates).OrderBy(c => c).ShouldBe(expectedCoordinatesForNoComboTile);

        var twoTilesCombination = TilesCombination(2, playReturns);
        twoTilesCombination.Count.ShouldBe(expectedFirstCoordinatesForTwoTilesCombination.Count);
        twoTilesCombination.Select(comb => comb.Min(t => t.Coordinates)).OrderBy(c => c).ShouldBe(expectedFirstCoordinatesForTwoTilesCombination.OrderBy(c => c));
        twoTilesCombination.Select(comb => comb.Max(t => t.Coordinates)).OrderBy(c => c).ShouldBe(expectedSecondCoordinatesForTwoTilesCombination.OrderBy(c => c));
        //twoTilesCombination.Select(comb => comb.Select(t => t.Coordinates)).OrderBy(c => c).ShouldBe(expectedCoordinatesForTwoTilesCombination.OrderBy(c => c));
    }

    [Fact]
    public void BoardNotEmptyScenario04()
    {
        var gameId = _player.GameId;
        var constTile0 = Tile(TileShape.Square, TileColor.Orange);
        var constTile1 = Tile(TileShape.Square, TileColor.Blue);
        var constTile2 = Tile(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = Tile(TileShape.Circle, TileColor.Red);
        var constTile4 = Tile(TileShape.Circle, TileColor.Yellow);
        var constTile5 = Tile(TileShape.Diamond, TileColor.Purple);

        var playerTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, playerTiles);

        //board construction
        var tilesOnBoard = new List<TileOnBoardDao>
        {
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Red), PositionX = 10, PositionY = 49},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Green), PositionX = 10, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow), PositionX = 11, PositionY = 50},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Yellow, 1), PositionX = 10, PositionY = 51},
            new() {GameId = gameId, TileId = TileId(TileShape.Circle, TileColor.Red, 1), PositionX = 11, PositionY = 51},
        };
        _dbContext.TilesOnBoard.AddRange(tilesOnBoard);
        _dbContext.SaveChanges();

        var expectedCoordinatesForNoComboTile = new List<Coordinates>
        {
            Coordinates.From(9, 49), Coordinates.From(9, 50), Coordinates.From(12, 50)
        };

        var expectedFirstCoordinatesForTwoTilesCombination = new List<Coordinates>
        {
            Coordinates.From(09, 48),Coordinates.From(09, 49), Coordinates.From(09, 49), Coordinates.From(12, 49),
        };

        var expectedSecondCoordinatesForTwoTilesCombination = new List<Coordinates>
        {
            Coordinates.From(9, 49), Coordinates.From(09, 50), Coordinates.From(9, 50), Coordinates.From(12, 50)
        };

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);
        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(expectedCoordinatesForNoComboTile.Count);
        noComboTile.Select(t => t.First().Coordinates).OrderBy(c => c).ShouldBe(expectedCoordinatesForNoComboTile.OrderBy(c => c));

        var twoTilesCombination = TilesCombination(2, playReturns);
        twoTilesCombination.Count.ShouldBe(expectedFirstCoordinatesForTwoTilesCombination.Count);

        var toto = twoTilesCombination.Select(t => t.First().Coordinates).OrderBy(c => c).ToList();
        twoTilesCombination.Select(comb => comb.Min(t => t.Coordinates)).OrderBy(c => c).ShouldBe(expectedFirstCoordinatesForTwoTilesCombination.OrderBy(c => c));
        twoTilesCombination.Select(comb => comb.Max(t => t.Coordinates)).OrderBy(c => c).ShouldBe(expectedSecondCoordinatesForTwoTilesCombination.OrderBy(c => c));

    }
}