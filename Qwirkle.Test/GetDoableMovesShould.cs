namespace Qwirkle.Test;

public class GetDoableMovesShould
{
    #region constructor and privates
    private readonly DefaultDbContext _dbContext;
    private readonly BotService _botService;
    private readonly Player _player;
    private readonly int _userId;

    public GetDoableMovesShould()
    {
        var connectionFactory = new ConnectionFactory();
        _dbContext = connectionFactory.CreateContextForInMemory();
        connectionFactory.Add4DefaultTestUsers();
        var repository = new Repository(_dbContext);
        var infoService = new InfoService(repository, null, new Logger<InfoService>(new LoggerFactory()));
        var coreService = new CoreService(repository, null, infoService, null, new Logger<CoreService>(new LoggerFactory()));
        var usersIds = infoService.GetAllUsersId();
        var gameId = coreService.CreateGame(usersIds.ToHashSet());
        var players = infoService.GetGame(gameId).Players.OrderBy(p => p.Id).ToList();
        _botService = new BotService(infoService, coreService, new Logger<CoreService>(new LoggerFactory()));
        _player = players[0];
        _userId = usersIds[0];
    }

    private void ChangePlayerTilesBy(int playerId, IReadOnlyList<TileDao> newTiles)
    {
        var tilesOnPlayer = _dbContext.TilesOnPlayer.Where(t => t.PlayerId == playerId).ToList();
        for (var i = 0; i < newTiles.Count; i++) tilesOnPlayer[i].TileId = newTiles[i].Id;
        for (var i = newTiles.Count; i < CoreService.TilesNumberPerPlayer; i++) _dbContext.TilesOnPlayer.Remove(tilesOnPlayer[i]);
        _dbContext.SaveChanges();
    }

    private static List<Move> TilesCombination(int tilesNumberInCombo, IEnumerable<PlayReturn> playReturns) => playReturns.Where(p => p.Move.Tiles.Count == tilesNumberInCombo).Select(p => p.Move).ToList();

    private int TileId(TileShape shape, TileColor color, int idIndex = 0) => TileDao(shape, color, idIndex).Id;

    private TileDao TileDao(TileShape shape, TileColor color, int idIndex = 0) => _dbContext.Tiles.Where(t => t.Shape == shape && t.Color == color).OrderBy(t => t.Id).AsEnumerable().ElementAt(idIndex);
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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack
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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(constTiles.Select(t => t.ToTile()).OrderBy(t => t)); // each of tile from rack

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
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldAllBe(c => c == Coordinates.From(0, 0)); // all in coordinates (0,0)
        //noComboTile.Select(t => t.First().ToTile()).OrderBy(t => t).ShouldBe(expectedTilesPlayable.Select(t => t.ToTile()).OrderBy(t => t));
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t)
            .ShouldBe(expectedTilesPlayable.Select(t => t.ToTile()).OrderBy(t => t));

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact(DisplayName = "Board non vide / 1 tile possible")]
    public void BoardNotEmptyScenario01()
    {
        var gameId = _player.GameId;
        var constTile0 = TileDao(TileShape.Circle, TileColor.Orange);
        var expectedTilePlayable = constTile0;
        var playerTiles = new List<TileDao> { constTile0 }.OrderBy(t => t.Id).ToList();
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
        noComboTile.Count.ShouldBe(expectedCoordinates.Count);
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldBe(expectedCoordinates.OrderBy(c => c));
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).OrderBy(t => t).ShouldAllBe(t => t == expectedTilePlayable.ToTile());

        TilesCombination(2, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(3, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(4, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(5, playReturns).Count.ShouldBe(0); // no combination possible
        TilesCombination(6, playReturns).Count.ShouldBe(0); // no combination possible
    }

    [Fact(DisplayName = "Board non vide / aucun move possible")]
    public void BoardNotEmptyScenario02()
    {
        var gameId = _player.GameId;
        var constTile0 = TileDao(TileShape.Square, TileColor.Orange);
        var constTile1 = TileDao(TileShape.Square, TileColor.Blue);
        var constTile2 = TileDao(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = TileDao(TileShape.FourPointStar, TileColor.Red);
        var constTile4 = TileDao(TileShape.Circle, TileColor.Yellow);
        var constTile5 = TileDao(TileShape.Diamond, TileColor.Purple);

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

    [Fact(DisplayName = "Board non vide / moves de 2 tiles possibles")]
    public void BoardNotEmptyScenario03()
    {
        var gameId = _player.GameId;
        var constTile0 = TileDao(TileShape.Square, TileColor.Orange);
        var constTile1 = TileDao(TileShape.Square, TileColor.Blue);
        var constTile2 = TileDao(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = TileDao(TileShape.Circle, TileColor.Red);
        var constTile4 = TileDao(TileShape.Circle, TileColor.Yellow);
        var constTile5 = TileDao(TileShape.Diamond, TileColor.Purple);

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

        var movesExpectedWith2Tiles = new List<Move>
        {
            new(
                new List<TileOnBoard>
                    {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 10, 49)}, 5),
            new(
                new List<TileOnBoard>
                    {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 52), TileOnBoard.From(TileShape.Circle, TileColor.Red, 10, 52)}, 5),
            new(
                new List<TileOnBoard>
                    {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 5),
            new(
                new List<TileOnBoard>
                    {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 12, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 12, 50)}, 5),
        };

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);
        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(expectedCoordinatesForNoComboTile.Count);
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.Coordinates).ShouldBe(expectedCoordinatesForNoComboTile);
        noComboTile.SelectMany(t => t.Tiles).Select(t => t.ToTile()).ShouldAllBe(t => t == new Tile(TileColor.Red, TileShape.Circle));

        var twoTilesCombination = TilesCombination(2, playReturns);
        twoTilesCombination.Count.ShouldBe(movesExpectedWith2Tiles.Count);
        twoTilesCombination.ShouldBeEquivalentTo(movesExpectedWith2Tiles);
    }

    [Fact(DisplayName = "Board non vide / moves de 2 tiles possibles")]
    public void BoardNotEmptyScenario04()
    {
        var gameId = _player.GameId;
        var constTile0 = TileDao(TileShape.Square, TileColor.Orange);
        var constTile1 = TileDao(TileShape.Square, TileColor.Blue);
        var constTile2 = TileDao(TileShape.EightPointStar, TileColor.Blue);
        var constTile3 = TileDao(TileShape.Circle, TileColor.Red);
        var constTile4 = TileDao(TileShape.Circle, TileColor.Yellow);
        var constTile5 = TileDao(TileShape.Diamond, TileColor.Purple);

        var playerTiles = new List<TileDao> { constTile0, constTile1, constTile2, constTile3, constTile4, constTile5 }.OrderBy(t => t.Id).ToList();
        ChangePlayerTilesBy(_player.Id, playerTiles);

        //board construction
        var tilesOnBoard = new List<TileOnBoardDao>
        {
            new() {TileId = TileId(TileShape.Circle, TileColor.Red),       PositionX = 10, PositionY = 49,GameId = gameId },
            new() {TileId = TileId(TileShape.Circle, TileColor.Green),     PositionX = 10, PositionY = 50,GameId = gameId },
            new() {TileId = TileId(TileShape.Circle, TileColor.Yellow),    PositionX = 11, PositionY = 50,GameId = gameId },
            new() {TileId = TileId(TileShape.Circle, TileColor.Yellow, 1), PositionX = 10, PositionY = 51,GameId = gameId },
            new() {TileId = TileId(TileShape.Circle, TileColor.Red, 1),    PositionX = 11, PositionY = 51,GameId = gameId },
        };
        _dbContext.TilesOnBoard.AddRange(tilesOnBoard);
        _dbContext.SaveChanges();

        var movesExpectedWith1Tile = new List<Move>
        {
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49)}, 2),
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 3),
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Red, 12, 50)}, 3),
        };

        var movesExpectedWith2Tiles = new List<Move>
        {
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 7),
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 48), TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49)}, 4),
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 7),
            new(new List<TileOnBoard> {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 12, 49), TileOnBoard.From(TileShape.Circle, TileColor.Red, 12, 50)}, 5),
        };

        var playReturns = _botService.ComputeDoableMoves(gameId, _userId);
        var noComboTile = TilesCombination(1, playReturns);
        noComboTile.Count.ShouldBe(movesExpectedWith1Tile.Count);
        noComboTile.ShouldBeEquivalentTo(movesExpectedWith1Tile);

        var twoTilesCombination = TilesCombination(2, playReturns);
        twoTilesCombination.Count.ShouldBe(movesExpectedWith2Tiles.Count);
        twoTilesCombination.ShouldBeEquivalentTo(movesExpectedWith2Tiles);
    }

    [Fact(DisplayName = "Board non vide / moves de 2 tiles possibles")]
    public void BoardNotEmptyScenario05()
    {
        var set0 = new HashSet<Move>
        {
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49)}, 2),
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 3),
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Purple, 12, 50)}, 4),
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Red, 12, 50)}, 5),
            new(new(){TileOnBoard.From(TileShape.Square, TileColor.Red, 12, 50)}, 6),
        };

        var set1 = new HashSet<Move>
        {
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Purple, 12, 50)}, 3),
            new(new(){TileOnBoard.From(TileShape.Square, TileColor.Red, 12, 50)}, 3),
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50)}, 3),
            new(new(){TileOnBoard.From(TileShape.Circle, TileColor.Red, 12, 50)}, 3),
            new(new() {TileOnBoard.From(TileShape.Circle, TileColor.Yellow, 9, 49)}, 2),
        };
        //var set0Ordered = set0.OrderBy(m => m).ToList();
        //var set1Ordered = set1.OrderBy(m => m).ToList();

        //set0Ordered.ShouldBeEquivalentTo(set1Ordered);
        //set0Ordered.ShouldBe(set1Ordered);

        Move move0 = new(new() { TileOnBoard.From(TileShape.Circle, TileColor.Red, 9, 50) }, 3);
        Move move1 = new(new() { TileOnBoard.From(TileShape.Clover, TileColor.Green, 10, 25) }, 3);
        Move move2 = new(new() { TileOnBoard.From(TileShape.Square, TileColor.Purple, 8, 40) }, 6);

        var hashSet0 = new HashSet<HashSet<Move>> { set0, set1 };
        var hashSet1 = new HashSet<HashSet<Move>> { set1, set0 };


    }
}

