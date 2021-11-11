using Qwirkle.Core.UseCases;

namespace Qwirkle.Core.Tests;

[Collection("Sequential")]
public class PlayTilesShould
{
    private readonly CoreUseCase _complianceService;

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

    public PlayTilesShould()
    {
        IRepository repository = new Repository(Context());
        _complianceService = new CoreUseCase(repository, null);
    }

    #region private methods
    private DefaultDbContext Context()
    {
        var contextOptions = new DbContextOptionsBuilder<DefaultDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new DefaultDbContext(contextOptions);
        AddAllTiles(dbContext);
        AddUsers(dbContext);
        AddGames(dbContext);
        AddPlayers(dbContext);
        AddTilesOnPlayers(dbContext);
        AddTilesOnBag(dbContext);
        return dbContext;
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
        for (int i = 1; i <= TotalTiles; i++)
            dbContext.TilesOnBag.Add(new TileOnBagDao { Id = 100 + i, GameId = GameId, TileId = i });

        dbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void Return3After1PlayerHavePlayedHisTiles()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay).Points.ShouldBe(3);
    }

    [Fact]
    public void Return0WhenItsNotTurnPlayer()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
        _complianceService.TryPlayTiles(Player3, tilesToPlay).Points.ShouldBe(0);
    }

    [Fact]
    public void Return0After1PlayerHavePlayedNotHisTiles()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (7, -3, 4) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay).Points.ShouldBe(0);
    }

    [Fact]
    public void Return5After2PlayersHavePlayed()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay);
        var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
        _complianceService.TryPlayTiles(Player3, tilesToPlay2).Points.ShouldBe(5);
    }

    [Fact]
    public void Return6After3PlayersHavePlayed()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay);
        var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
        _complianceService.TryPlayTiles(Player3, tilesToPlay2);
        var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
        Assert.Equal(6, _complianceService.TryPlayTiles(Player8, tilesToPlay3).Points);
    }

    [Fact]
    public void Return6After4PlayersHavePlayed()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay);
        var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
        _complianceService.TryPlayTiles(Player3, tilesToPlay2);
        var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
        _complianceService.TryPlayTiles(Player8, tilesToPlay3);
        var tilesToPlay4 = new List<(int tileId, sbyte x, sbyte y)> { (21, -3, 2), (20, -3, 1), (19, -3, 0) };
        _complianceService.TryPlayTiles(Player14, tilesToPlay4).Points.ShouldBe(6);
    }

    [Fact]
    public void Return0AfterPlayersHavePlayedOnTheSamePlaceThanOtherTile()
    {
        var tilesToPlay = new List<(int tileId, sbyte x, sbyte y)> { (1, -3, 4), (2, -3, 5), (3, -3, 6) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay);
        var tilesToPlay2 = new List<(int tileId, sbyte x, sbyte y)> { (7, -4, 4), (8, -4, 3), (9, -4, 2) };
        _complianceService.TryPlayTiles(Player3, tilesToPlay2);
        var tilesToPlay3 = new List<(int tileId, sbyte x, sbyte y)> { (13, -2, 4), (14, -2, 3), (15, -2, 2) };
        _complianceService.TryPlayTiles(Player8, tilesToPlay3);
        var tilesToPlay4 = new List<(int tileId, sbyte x, sbyte y)> { (21, -3, 2), (20, -3, 1), (19, -3, 0) };
        _complianceService.TryPlayTiles(Player14, tilesToPlay4);
        var tilesToPlay5 = new List<(int tileId, sbyte x, sbyte y)> { (4, -3, 2), (5, -3, 1), (6, -3, 0) };
        _complianceService.TryPlayTiles(Player9, tilesToPlay5).Points.ShouldBe(0);
    }

}
