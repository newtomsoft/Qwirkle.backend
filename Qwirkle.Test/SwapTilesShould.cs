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

    private void InitTest()
    {
        InitDbContext();
        InitData();
        _repository = new Repository(_dbContext);
        _infoService = new InfoService(_repository, null, new Logger<InfoService>(new LoggerFactory()));
        _coreService = new CoreService(_repository, new NoNotification(), _infoService, null, new Logger<CoreService>(new LoggerFactory()));
    }

    private void InitDbContext()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new DefaultDbContext(contextOptions);
    }

    private void InitData()
    {
        AddAllTiles();
        AddUsers();
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
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.First(p => p.IsTurn is false);
        var swapReturn = _coreService.TrySwapTiles(player.Id, new List<Tile> { player.Rack.Tiles[0] });
        swapReturn.Code.ShouldBe(ReturnCode.NotPlayerTurn);
    }

    [Fact]
    public void ReturnPlayerDoesntHaveThisTileAfter1PlayerHaveSwapTiles()
    {
        InitTest();
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.Single(p => p.IsTurn);
        Tile? tileToSwap;
        do
        {
            var tileToSwapDao = _dbContext.TilesOnBag.Include(t => t.Tile).Where(t => t.GameId == player.GameId).AsEnumerable().OrderBy(_ => Guid.NewGuid()).First().Tile;
            tileToSwap = tileToSwapDao.ToTile();
        }
        while (player.Rack.Tiles.Any(t => t.Color == tileToSwap.Color && t.Shape == tileToSwap.Shape));
        var swapReturn = _coreService.TrySwapTiles(player.Id, new List<Tile> { tileToSwap });

        swapReturn.Code.ShouldBe(ReturnCode.PlayerDoesntHaveThisTile);
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap1Tile()
    {
        for (var i = 0; i < CoreService.TilesNumberPerPlayer; i++)
        {
            InitTest();
            var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
            var players = _infoService.GetGame(gameId).Players;
            var player = players.Single(p => p.IsTurn);

            var tileToSwap = player.Rack.Tiles[i];
            var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
            oldRackWithoutSwappedTile.Remove(tileToSwap);
            var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

            var tilesToSwap = new List<Tile> { tileToSwap };
            var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
            swapReturn.Code.ShouldBe(ReturnCode.Ok);

            var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
            var newTiles = new List<Tile>(tilesInRack);

            foreach (var tile in oldTilesWithoutSwappedTile)
                newTiles.Remove(tile);

            newTiles.Count.ShouldBe(tilesToSwap.Count);
        }
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap2Tiles()
    {
        for (var firstTileIndex = 0; firstTileIndex < CoreService.TilesNumberPerPlayer; firstTileIndex++)
        {
            for (var secondTileIndex = firstTileIndex + 1; secondTileIndex < CoreService.TilesNumberPerPlayer; secondTileIndex++)
            {
                InitTest();
                var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
                var players = _infoService.GetGame(gameId).Players;
                var player = players.Single(p => p.IsTurn);

                var tileToSwap0 = player.Rack.Tiles[firstTileIndex];
                var tileToSwap1 = player.Rack.Tiles[secondTileIndex];

                var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
                oldRackWithoutSwappedTile.Remove(tileToSwap0);
                oldRackWithoutSwappedTile.Remove(tileToSwap1);
                var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

                var tilesToSwap = new List<Tile> { tileToSwap0, tileToSwap1 };
                var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
                swapReturn.Code.ShouldBe(ReturnCode.Ok);

                var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
                var newTiles = new List<Tile>(tilesInRack);

                foreach (var tile in oldTilesWithoutSwappedTile)
                    newTiles.Remove(tile);

                newTiles.Count.ShouldBe(tilesToSwap.Count);
            }
        }
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap3Tiles()
    {
        for (var firstTileIndex = 0; firstTileIndex < CoreService.TilesNumberPerPlayer; firstTileIndex++)
        {
            for (var secondTileIndex = firstTileIndex + 1; secondTileIndex < CoreService.TilesNumberPerPlayer; secondTileIndex++)
            {
                for (var thirdTileIndex = secondTileIndex + 1; thirdTileIndex < CoreService.TilesNumberPerPlayer; thirdTileIndex++)
                {
                    InitTest();
                    var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
                    var players = _infoService.GetGame(gameId).Players;
                    var player = players.Single(p => p.IsTurn);

                    var tileToSwap0 = player.Rack.Tiles[firstTileIndex];
                    var tileToSwap1 = player.Rack.Tiles[secondTileIndex];
                    var tileToSwap2 = player.Rack.Tiles[thirdTileIndex];

                    var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
                    oldRackWithoutSwappedTile.Remove(tileToSwap0);
                    oldRackWithoutSwappedTile.Remove(tileToSwap1);
                    oldRackWithoutSwappedTile.Remove(tileToSwap2);
                    var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

                    var tilesToSwap = new List<Tile> { tileToSwap0, tileToSwap1, tileToSwap2 };
                    var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
                    swapReturn.Code.ShouldBe(ReturnCode.Ok);

                    var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
                    var newTiles = new List<Tile>(tilesInRack);

                    foreach (var tile in oldTilesWithoutSwappedTile)
                        newTiles.Remove(tile);

                    newTiles.Count.ShouldBe(tilesToSwap.Count);
                }
            }
        }
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap4Tiles()
    {
        for (var firstTileIndex = 0; firstTileIndex < CoreService.TilesNumberPerPlayer; firstTileIndex++)
        {
            for (var secondTileIndex = firstTileIndex + 1; secondTileIndex < CoreService.TilesNumberPerPlayer; secondTileIndex++)
            {
                for (var thirdTileIndex = secondTileIndex + 1; thirdTileIndex < CoreService.TilesNumberPerPlayer; thirdTileIndex++)
                {
                    for (var fourthTileIndex = thirdTileIndex + 1; fourthTileIndex < CoreService.TilesNumberPerPlayer; fourthTileIndex++)
                    {
                        InitTest();
                        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
                        var players = _infoService.GetGame(gameId).Players;
                        var player = players.Single(p => p.IsTurn);

                        var tileToSwap0 = player.Rack.Tiles[firstTileIndex];
                        var tileToSwap1 = player.Rack.Tiles[secondTileIndex];
                        var tileToSwap2 = player.Rack.Tiles[thirdTileIndex];
                        var tileToSwap3 = player.Rack.Tiles[fourthTileIndex];

                        var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
                        oldRackWithoutSwappedTile.Remove(tileToSwap0);
                        oldRackWithoutSwappedTile.Remove(tileToSwap1);
                        oldRackWithoutSwappedTile.Remove(tileToSwap2);
                        oldRackWithoutSwappedTile.Remove(tileToSwap3);
                        var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

                        var tilesToSwap = new List<Tile> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3 };
                        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
                        swapReturn.Code.ShouldBe(ReturnCode.Ok);

                        var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
                        var newTiles = new List<Tile>(tilesInRack);

                        foreach (var tile in oldTilesWithoutSwappedTile)
                            newTiles.Remove(tile);

                        newTiles.Count.ShouldBe(tilesToSwap.Count);
                    }
                }
            }
        }
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap5Tiles()
    {
        for (var firstTileIndex = 0; firstTileIndex < CoreService.TilesNumberPerPlayer; firstTileIndex++)
        {
            for (var secondTileIndex = firstTileIndex + 1; secondTileIndex < CoreService.TilesNumberPerPlayer; secondTileIndex++)
            {
                for (var thirdTileIndex = secondTileIndex + 1; thirdTileIndex < CoreService.TilesNumberPerPlayer; thirdTileIndex++)
                {
                    for (var fourthTileIndex = thirdTileIndex + 1; fourthTileIndex < CoreService.TilesNumberPerPlayer; fourthTileIndex++)
                    {
                        for (var fifthTileIndex = fourthTileIndex + 1; fifthTileIndex < CoreService.TilesNumberPerPlayer; fifthTileIndex++)
                        {
                            InitTest();
                            var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
                            var players = _infoService.GetGame(gameId).Players;
                            var player = players.Single(p => p.IsTurn);

                            var tileToSwap0 = player.Rack.Tiles[firstTileIndex];
                            var tileToSwap1 = player.Rack.Tiles[secondTileIndex];
                            var tileToSwap2 = player.Rack.Tiles[thirdTileIndex];
                            var tileToSwap3 = player.Rack.Tiles[fourthTileIndex];
                            var tileToSwap4 = player.Rack.Tiles[fifthTileIndex];

                            var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
                            oldRackWithoutSwappedTile.Remove(tileToSwap0);
                            oldRackWithoutSwappedTile.Remove(tileToSwap1);
                            oldRackWithoutSwappedTile.Remove(tileToSwap2);
                            oldRackWithoutSwappedTile.Remove(tileToSwap3);
                            oldRackWithoutSwappedTile.Remove(tileToSwap4);
                            var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

                            var tilesToSwap = new List<Tile> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3, tileToSwap4 };
                            var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
                            swapReturn.Code.ShouldBe(ReturnCode.Ok);

                            var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
                            var newTiles = new List<Tile>(tilesInRack);

                            foreach (var tile in oldTilesWithoutSwappedTile)
                                newTiles.Remove(tile);

                            newTiles.Count.ShouldBe(tilesToSwap.Count);
                        }
                    }
                }
            }
        }
    }

    [Fact]
    public void ReturnOkAfter1PlayerHaveSwap6Tiles()
    {
        InitTest();
        var gameId = _coreService.CreateGame(new HashSet<int> { User0Id, User1Id });
        var players = _infoService.GetGame(gameId).Players;
        var player = players.Single(p => p.IsTurn);

        var tileToSwap0 = player.Rack.Tiles[0];
        var tileToSwap1 = player.Rack.Tiles[1];
        var tileToSwap2 = player.Rack.Tiles[2];
        var tileToSwap3 = player.Rack.Tiles[3];
        var tileToSwap4 = player.Rack.Tiles[4];
        var tileToSwap5 = player.Rack.Tiles[5];
        var tilesToSwap = new List<Tile> { tileToSwap0, tileToSwap1, tileToSwap2, tileToSwap3, tileToSwap4, tileToSwap5 };

        var oldRackWithoutSwappedTile = new List<TileOnPlayer>(player.Rack.Tiles);
        oldRackWithoutSwappedTile.Remove(tileToSwap0);
        oldRackWithoutSwappedTile.Remove(tileToSwap1);
        oldRackWithoutSwappedTile.Remove(tileToSwap2);
        oldRackWithoutSwappedTile.Remove(tileToSwap3);
        oldRackWithoutSwappedTile.Remove(tileToSwap4);
        oldRackWithoutSwappedTile.Remove(tileToSwap5);

        var oldTilesWithoutSwappedTile = oldRackWithoutSwappedTile.Select(t => t.ToTile()).OrderBy(t => t).ToList();

        var swapReturn = _coreService.TrySwapTiles(player.Id, tilesToSwap);
        swapReturn.Code.ShouldBe(ReturnCode.Ok);

        var tilesInRack = swapReturn.NewRack.Tiles.Select(t => t.ToTile()).ToList();
        var newTiles = new List<Tile>(tilesInRack);
        foreach (var tile in oldTilesWithoutSwappedTile) newTiles.Remove(tile);

        newTiles.Count.ShouldBe(tilesToSwap.Count);
    }
}
