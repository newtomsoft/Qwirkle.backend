namespace ComplianceRepository.Test;

public class RepositoryTest
{
    #region private attributs
    private IRepository Repository { get; set; }
    private DefaultDbContext DbContext { get; set; }

    private const int USER1 = 71;
    private const int USER2 = 21;
    private const int USER3 = 3;
    private const int USER4 = 14;
    #endregion

    public RepositoryTest()
    {
        var factory = new ConnectionFactory();
        DbContext = factory.CreateContextForInMemory();
        Repository = new Repository(DbContext);
        AddUsers();
        AddGame();
        AddAllTiles();
    }

    #region private methods
    private void AddUsers()
    {
        DbContext.Users.Add(new UserDao { Id = USER1 });
        DbContext.Users.Add(new UserDao { Id = USER2 });
        DbContext.Users.Add(new UserDao { Id = USER3 });
        DbContext.Users.Add(new UserDao { Id = USER4 });
        DbContext.SaveChanges();
    }

    private void AddGame()
    {
        DbContext.Games.Add(new GameDao { CreatDate = DateTime.Now });
        DbContext.SaveChanges();
    }

    private void AddAllTiles()
    {
        const int NUMBER_OF_SAME_TILE = 3;
        for (int i = 0; i < NUMBER_OF_SAME_TILE; i++)
            foreach (var color in (TileColor[])Enum.GetValues(typeof(TileColor)))
                foreach (var shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
                    DbContext.Tiles.Add(new TileDao { Color = color, Shape = shape });
        DbContext.SaveChanges();
    }

    private void AddTilesOnBag(int gameId, int number)
    {
        var tilesIds = DbContext.Tiles.Select(t => t.Id).ToList();
        for (int i = 1; i <= number; i++)
            DbContext.TilesOnBag.Add(new TileOnBagDao { GameId = gameId, TileId = tilesIds[i] });
        DbContext.SaveChanges();
    }
    #endregion

    [Fact]
    public void CreatePlayerShould()
    {
        var game = DbContext.Games.First();
        var player = Repository.CreatePlayer(USER1, game.Id);
        Assert.Equal(game.Id, player.GameId);
        Assert.Equal(0, player.GamePosition);
        Assert.False(player.IsTurn);
        Assert.Equal(0, player.Points);
        Assert.Equal(0, player.LastTurnPoints);
        Assert.Empty(player.Rack.Tiles);
    }

    [Fact]
    public void CreateGameShould()
    {
        var game = Repository.CreateGame(DateTime.Today);
        game.Board.Tiles.ShouldBeEmpty();
        game.Players.ShouldBeEmpty();
    }

    [Fact]
    public void UpdatePlayerShould()
    {
        byte points = 10;
        byte position = 2;
        byte lastTurnPoints = 3;
        var game = Repository.CreateGame(DateTime.Today);
        var player = Repository.CreatePlayer(USER1, game.Id);
        player.Points = points;
        player.LastTurnPoints = lastTurnPoints;
        player.SetTurn(true);
        player.GamePosition = position;
        Repository.UpdatePlayer(player);
        var playerUpdate = Repository.GetPlayer(player.Id);
        playerUpdate.Points.ShouldBe(points);
        playerUpdate.LastTurnPoints.ShouldBe(lastTurnPoints);
        playerUpdate.IsTurn.ShouldBeTrue();
        playerUpdate.GamePosition.ShouldBe(position);
    }

    [Fact]
    public void TilesFromBagToPlayerShouldNotGiveTileIfBagIsEmpty()
    {
        var game = Repository.CreateGame(DateTime.Today);
        var player = Repository.CreatePlayer(USER1, game.Id);
        Repository.TilesFromBagToPlayer(player, new List<byte> { 0, 1, 2, 3, 4, 5 });
        var playerUpdate = Repository.GetPlayer(player.Id);
        playerUpdate.Rack.Tiles.ShouldBeEmpty();
    }

    [Fact]
    public void TilesFromBagToPlayerShouldGiveTilesIfBagContainEnoughTiles()
    {
        int tilesNumberToAdd = 3;
        var rackPositions = new List<byte>();
        for (byte i = 0; i < tilesNumberToAdd; i++)
        {
            rackPositions.Add(i);
        }
        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, tilesNumberToAdd);
        var player = Repository.CreatePlayer(USER1, game.Id);
        Repository.TilesFromBagToPlayer(player, rackPositions);
        var playerUpdate = Repository.GetPlayer(player.Id);
        playerUpdate.Rack.Tiles.Count.ShouldBe(tilesNumberToAdd);
    }

    [Fact]
    public void TilesFromBagToPlayerShouldGive4TilesWhenRequest2x2()
    {
        int tilesNumberToAddInBag = 10;
        int tilesNumberToRequest = 2;
        var rackPositions = new List<byte>();
        for (byte i = 0; i < tilesNumberToRequest; i++)
            rackPositions.Add(i);

        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, tilesNumberToAddInBag);
        var player = Repository.CreatePlayer(USER1, game.Id);
        Repository.TilesFromBagToPlayer(player, rackPositions);
        Repository.TilesFromBagToPlayer(player, rackPositions);
        var playerUpdate = Repository.GetPlayer(player.Id);
        playerUpdate.Rack.Tiles.Count.ShouldBe(tilesNumberToRequest + tilesNumberToRequest);
    }

    [Fact]
    public void TilesFromPlayerToBagAllShouldEmptyPlayer()
    {
        int tilesNumberToAddInBag = 10;
        int tilesNumberToRequestFromBag = 4;
        var rackPositions = new List<byte>();
        for (byte i = 0; i < tilesNumberToRequestFromBag; i++)
            rackPositions.Add(i);
        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, tilesNumberToAddInBag);
        var player = Repository.CreatePlayer(USER1, game.Id);
        Repository.TilesFromBagToPlayer(player, rackPositions);
        var playerAfterDraw = Repository.GetPlayer(player.Id);
        Repository.TilesFromPlayerToBag(playerAfterDraw, playerAfterDraw.Rack.Tiles);
        var playerUpdate = Repository.GetPlayer(player.Id);
        var gameUpdate = Repository.GetGame(game.Id);
        playerUpdate.Rack.Tiles.ShouldBeEmpty();
        gameUpdate.Bag.Tiles.Count.ShouldBe(tilesNumberToAddInBag);
    }

    [Fact]
    public void GetGameShouldContainPlayer()
    {
        int TilesNumberToAddInBag = 10;
        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, TilesNumberToAddInBag);
        var player1 = Repository.CreatePlayer(USER1, game.Id);
        var player2 = Repository.CreatePlayer(USER2, game.Id);
        var player3 = Repository.CreatePlayer(USER3, game.Id);
        var player4 = Repository.CreatePlayer(USER4, game.Id);
        var gameUpdate = Repository.GetGame(game.Id);
        gameUpdate.Players.Select(p => p.Id).ShouldContain(player1.Id);
        gameUpdate.Players.Select(p => p.Id).ShouldContain(player2.Id);
        gameUpdate.Players.Select(p => p.Id).ShouldContain(player3.Id);
        gameUpdate.Players.Select(p => p.Id).ShouldContain(player4.Id);
    }

    [Fact]
    public void GetGameShouldContainBag()
    {
        int tilesNumberToAddInBag = 10;
        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, tilesNumberToAddInBag);
        var gameUpdate = Repository.GetGame(game.Id);
        gameUpdate.Bag.Tiles.Count.ShouldBe(tilesNumberToAddInBag);
    }

    [Fact]
    public void TilesFromPlayerToGameShould()
    {
        int TilesNumberToAddInBag = 10;
        int TilesNumberToRequestFromBag = 4;
        var rackPositions = new List<byte>();
        for (byte i = 0; i < TilesNumberToRequestFromBag; i++)
        {
            rackPositions.Add(i);
        }
        var game = Repository.CreateGame(DateTime.Today);
        AddTilesOnBag(game.Id, TilesNumberToAddInBag);
        var player = Repository.CreatePlayer(USER1, game.Id);
        Repository.TilesFromBagToPlayer(player, rackPositions);
        player = Repository.GetPlayer(player.Id);
        var coordinates = new CoordinatesInGame(2, 5);
        var tile = new TileOnBoard(player.Rack.Tiles[0], coordinates);
        Repository.TilesFromPlayerToGame(game.Id, player.Id, new List<TileOnBoard> { tile });
        var gameUpdate = Repository.GetGame(game.Id);
        gameUpdate.Board.Tiles.ShouldHaveSingleItem();
        gameUpdate.Board.Tiles[0].Coordinates.ShouldBe(coordinates);
    }
}
