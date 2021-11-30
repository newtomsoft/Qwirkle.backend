namespace Qwirkle.Test;

//[Collection("Sequential")]
public class PlayTilesShould
{
    #region private
    private const int TotalTiles = 108;
    private const int GameId = 7;
    private const int User71 = 71;
    private const int User21 = 21;
    private const int User3 = 3;
    private const int User14 = 14;
    private const int Player9 = 9;
    private const int Player3 = 3;
    private const int Player8 = 8;
    private const int Player14 = 14;
    private DefaultDbContext Context()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DefaultDbContext(contextOptions);
        InitializeDatas();
        return dbContext;

        void InitializeDatas()
        {
            AddAllTiles(dbContext);
            AddUsers(dbContext);
            AddGames(dbContext);
            AddPlayers(dbContext);
            AddTilesOnPlayers(dbContext);
            AddTilesOnBag(dbContext);
        }
    }

    private static void AddAllTiles(DefaultDbContext dbContext)
    {
        const int numberOfSameTile = 3;
        var id = 0;
        for (var i = 0; i < numberOfSameTile; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    dbContext.Tiles.Add(new TileDao { Id = ++id, Color = color, Shape = shape });

        dbContext.SaveChanges();
    }

    private static void AddGames(DefaultDbContext dbContext)
    {
        dbContext.Games.Add(new GameDao { Id = GameId, });
        dbContext.SaveChanges();
    }

    private static void AddUsers(DefaultDbContext dbContext)
    {
        dbContext.Users.Add(new UserDao { Id = User71 });
        dbContext.Users.Add(new UserDao { Id = User21 });
        dbContext.Users.Add(new UserDao { Id = User3 });
        dbContext.Users.Add(new UserDao { Id = User14 });
        dbContext.SaveChanges();
    }

    private static void AddPlayers(DefaultDbContext dbContext)
    {
        dbContext.Players.Add(new PlayerDao { Id = Player9, UserId = User71, GameId = GameId, GamePosition = 1, GameTurn = true });
        dbContext.Players.Add(new PlayerDao { Id = Player3, UserId = User21, GameId = GameId, GamePosition = 2, GameTurn = false });
        dbContext.Players.Add(new PlayerDao { Id = Player8, UserId = User3, GameId = GameId, GamePosition = 3, GameTurn = false });
        dbContext.Players.Add(new PlayerDao { Id = Player14, UserId = User14, GameId = GameId, GamePosition = 4, GameTurn = false });
        dbContext.SaveChanges();
    }

    private static void AddTilesOnPlayers(DefaultDbContext dbContext)
    {
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 1, PlayerId = Player9, TileId = 1 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 2, PlayerId = Player9, TileId = 2 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 3, PlayerId = Player9, TileId = 3 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 4, PlayerId = Player9, TileId = 4 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 5, PlayerId = Player9, TileId = 5 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 6, PlayerId = Player9, TileId = 6 });

        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 11, PlayerId = Player3, TileId = 7 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 12, PlayerId = Player3, TileId = 8 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 13, PlayerId = Player3, TileId = 9 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 14, PlayerId = Player3, TileId = 10 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 15, PlayerId = Player3, TileId = 11 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 16, PlayerId = Player3, TileId = 12 });

        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 21, PlayerId = Player8, TileId = 13 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 22, PlayerId = Player8, TileId = 14 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 23, PlayerId = Player8, TileId = 15 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 24, PlayerId = Player8, TileId = 16 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 25, PlayerId = Player8, TileId = 17 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 26, PlayerId = Player8, TileId = 18 });

        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 31, PlayerId = Player14, TileId = 19 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 32, PlayerId = Player14, TileId = 20 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 33, PlayerId = Player14, TileId = 21 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 34, PlayerId = Player14, TileId = 22 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 35, PlayerId = Player14, TileId = 23 });
        dbContext.TilesOnPlayer.Add(new TileOnPlayerDao { Id = 36, PlayerId = Player14, TileId = 24 });

        dbContext.SaveChanges();
    }

    private void AddTilesOnBag(DefaultDbContext dbContext)
    {
        for (var i = 1; i <= TotalTiles; i++)
            dbContext.TilesOnBag.Add(new TileOnBagDao { Id = 100 + i, GameId = GameId, TileId = i });

        dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void Return0WhenItsNotTurnPlayer()
    {
        var dbContext = Context();
        var coreUseCase = new CoreUseCase(new Repository(dbContext), null);
        var tilesToPlay = new List<(int tileId, Coordinates coordinates)> { (7, Coordinates.From(-4, 4)), (8, Coordinates.From(-4, 3)), (9, Coordinates.From(-4, 2)) };
        coreUseCase.TryPlayTiles(Player3, tilesToPlay).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0After1PlayerHavePlayedNotHisTiles()
    {
        var dbContext = Context();
        var coreUseCase = new CoreUseCase(new Repository(dbContext), null);
        var tilesToPlay = new List<(int tileId, Coordinates coordinates)> { (7, Coordinates.From(-3, 4)) };
        coreUseCase.TryPlayTiles(Player9, tilesToPlay).Points.ShouldBe(0);
    }

    [Fact]
    public void Return3After1PlayerHavePlayedHisTiles()
    {
        var dbContext = Context();
        var coreUseCase = new CoreUseCase(new Repository(dbContext), null);
        var tilesToPlay = new List<(int tileId, Coordinates coordinates)> { (1, Coordinates.From(-3, 4)), (2, Coordinates.From(-3, 5)), (3, Coordinates.From(-3, 6)) };
        coreUseCase.TryPlayTiles(Player9, tilesToPlay).Points.ShouldBe(3);
    }

    [Fact]
    public void Return5After2PlayersHavePlayed()
    {
        var dbContext = Context();
        var coreUseCase = new CoreUseCase(new Repository(dbContext), null);
        var tilesToPlay = new List<(int tileId, Coordinates coordinates)> { (1, Coordinates.From(-3, 4)), (2, Coordinates.From(-3, 5)), (3, Coordinates.From(-3, 6)) };
        InitBoard();
        coreUseCase.TryPlayTiles(Player9, tilesToPlay).Points.ShouldBe(5); 

        void InitBoard()
        {
            dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 7, PositionX = -4, PositionY = 4 });
            dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 8, PositionX = -4, PositionY = 3 });
            dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 9, PositionX = -4, PositionY = 2 });
            dbContext.SaveChanges();
        }
    }
    
    [Fact]
    public void ReturnNotFreeWhenCoordinateOnBoardIsNotFree()
    {
        var dbContext = Context();
        var coreUseCase = new CoreUseCase(new Repository(dbContext), null);
        InitBoard();

        var tilesToPlay = new List<(int tileId, Coordinates coordinates)> { (1, Coordinates.From(5, 7)) };
        coreUseCase.TryPlayTiles(Player9, tilesToPlay).Code.ShouldBe(PlayReturnCode.NotFree);

        void InitBoard()
        {
            dbContext.TilesOnBoard.Add(new TileOnBoardDao { GameId = GameId, TileId = 69, PositionX = 5, PositionY = 7 });
            dbContext.SaveChanges();
        }
    }
}
