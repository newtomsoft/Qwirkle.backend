namespace Qwirkle.Test;

public class SwapTilesShould
{
    #region private

    private DefaultDbContext _dbContext = null!;
    private Repository _repository = null!;
    private InfoService _infoService = null!;
    private CoreService _coreService = null!;

    private const int User0Id = 71;
    private const int User1Id = 72;
    private const int Player0Id = 9;



    private void InitDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DefaultDbContext(contextOptions);
        InitializeData();
    }

    private void InitializeData()
    {
        AddAllTiles();
        AddUsers();
    }

    private void InitTest()
    {
        InitDbContext();
        _repository = new Repository(_dbContext);
        var authenticationUseCase = new UserService(new NoRepository(), new FakeAuthentication());
        _infoService = new InfoService(_repository, null, new Logger<InfoService>(new LoggerFactory()));
        _coreService = new CoreService(_repository, new NoNotification(), _infoService, authenticationUseCase, new Logger<CoreService>(new LoggerFactory()));
    }

    private void AddAllTiles()
    {
        const int numberOfSameTile = 3;
        var id = 0;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    _dbContext.Tiles.Add(new TileDao { Id = ++id, Color = color, Shape = shape });

        _dbContext.SaveChanges();
    }

    private void AddUsers()
    {
        _dbContext.Users.Add(new UserDao { Id = User0Id });
        _dbContext.Users.Add(new UserDao { Id = User1Id });
        _dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void ReturnNotPlayerTurnWhenItsNotTurnPlayer()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[1];
        var swapReturn = _coreService.TrySwapTiles(player.Id, new List<Tile> { player.Rack.Tiles[0] });
        swapReturn.Code.ShouldBe(PlayReturnCode.NotPlayerTurn);
    }

    [Fact]
    public void ReturnPlayerDoesntHaveThisTileAfter1PlayerHaveSwapTiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];
        Tile? tileToSwap;
        do
        {
            var tileToSwapDao = _dbContext.TilesOnBag.Include(t => t.Tile).Where(t => t.GameId == player.GameId).AsEnumerable().OrderBy(_ => Guid.NewGuid()).First().Tile;
            tileToSwap = tileToSwapDao.ToTile();
        }
        while (player.Rack.Tiles.Any(t => t.Color == tileToSwap.Color && t.Shape == tileToSwap.Shape));
        var swapReturn = _coreService.TrySwapTiles(player.Id, new List<Tile> { tileToSwap });

        swapReturn.Code.ShouldBe(PlayReturnCode.PlayerDoesntHaveThisTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap1Tile()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap = player.Rack.Tiles[0];
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, new List<Tile> { tileToSwap });
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap2Tiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tilesToSwap = new List<TileOnPlayer> { tileToSwap0, tileToSwap1 };
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap0 && t != tileToSwap1).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0 && t.RackPosition != 1).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }
    
    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap3Tiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tileToSwap2 = player.Rack.Tiles[2];
        var tilesToSwap = new List<TileOnPlayer> { tileToSwap0, tileToSwap1, tileToSwap2 };
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap0 && t != tileToSwap1 && t != tileToSwap2).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0 && t.RackPosition != 1 && t.RackPosition != 2).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap4Tiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tileToSwap2 = player.Rack.Tiles[2];
        var tileToSwap3 = player.Rack.Tiles[3];
        var tilesToSwap = new List<TileOnPlayer> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3 };
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap0 && t != tileToSwap1 && t != tileToSwap2 && t != tileToSwap3).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0 && t.RackPosition != 1 && t.RackPosition != 2 && t.RackPosition != 3).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap5Tiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tileToSwap2 = player.Rack.Tiles[2];
        var tileToSwap3 = player.Rack.Tiles[3];
        var tileToSwap4 = player.Rack.Tiles[4];
        var tilesToSwap = new List<TileOnPlayer> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3, tileToSwap4 };
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap0 && t != tileToSwap1 && t != tileToSwap2 && t != tileToSwap3 && t != tileToSwap4).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0 && t.RackPosition != 1 && t.RackPosition != 2 && t.RackPosition != 3 && t.RackPosition != 4).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap6Tiles()
    {
        InitTest();
        var players = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var player = players[0];

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tileToSwap2 = player.Rack.Tiles[2];
        var tileToSwap3 = player.Rack.Tiles[3];
        var tileToSwap4 = player.Rack.Tiles[4];
        var tileToSwap5 = player.Rack.Tiles[5];
        var tilesToSwap = new List<TileOnPlayer> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3, tileToSwap4, tileToSwap5 };
        var rackWithoutSwappedTile = player.Rack.Tiles.Where(t => t != tileToSwap0 && t != tileToSwap1 && t != tileToSwap2 && t != tileToSwap3 && t != tileToSwap4 && t != tileToSwap5).OrderBy(t => t.RackPosition).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(PlayReturnCode.Ok);

        var newRackWithoutNewTile = swapReturn.NewRack.Tiles.Where(t => t.RackPosition != 0 && t.RackPosition != 1 && t.RackPosition != 2 && t.RackPosition != 3 && t.RackPosition != 4 && t.RackPosition != 5).OrderBy(t => t.RackPosition).ToList();
        newRackWithoutNewTile.ShouldBe(rackWithoutSwappedTile);
    }
}
