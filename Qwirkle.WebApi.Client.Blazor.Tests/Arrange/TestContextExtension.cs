namespace Qwirkle.WebApi.Client.Blazor.Tests.Arrange;

public static class TestContextExtension
{
    public static void AddGenericServices(this TestContext context)
    {
        context.AddTestAuthorization().SetAuthorized("userName");

        context.Services.AddMockedService(new Mock<INotificationGame>());
        context.Services.AddMockedService(new Mock<INotificationInstantGame>());
        context.Services.AddMockedService(new Mock<INotificationReceiver>());
        context.Services.AddMockedService(new Mock<IApiAction>());

        var playersDetail = new Mock<IPlayersDetail>();
        playersDetail.Setup(x => x.All).Returns(new PlayerDetail[] { new(Player1), new(Player2) });
        context.Services.AddMockedService(playersDetail);

        var areaManager = new Mock<IAreaManager>();
        areaManager.Setup(x => x.BoardLimit).Returns(new BoardLimit(new List<Coordinate> { Coordinate.From(0, 0) }));
        context.Services.AddMockedService(areaManager);

        var dragNDropManager = new Mock<IDragNDropManager>();
        dragNDropManager.Setup(x => x.AllPlayerTiles).Returns(new HashSet<DropItem>());
        dragNDropManager.Setup(x => x.TilesDroppedOnBoard).Returns(new HashSet<DropItem> { new() { Coordinate = Coordinate.From(0, 0), RackPosition = 0, DropZone = DropZone.Board, Tile = new Tile(TileColor.Blue, TileShape.Clover) } });
        dragNDropManager.Setup(x => x.TilesDroppedOnBag).Returns(new HashSet<DropItem> { new() { RackPosition = 0, DropZone = DropZone.Bag, Tile = new Tile(TileColor.Blue, TileShape.Clover) } });
        dragNDropManager.Setup(x => x.AllPlayerTiles).Returns(new HashSet<DropItem> { new() { RackPosition = 0, DropZone = DropZone.Bag, Tile = new Tile(TileColor.Blue, TileShape.Clover) } });
        dragNDropManager.Setup(x => x.AllTilesInGame).Returns(new HashSet<DropItem> { new() { RackPosition = 0, DropZone = DropZone.Bag, Tile = new Tile(TileColor.Blue, TileShape.Clover) } });
        context.Services.AddMockedService(dragNDropManager);
        context.JSInterop.SetupVoid("mudDragAndDrop.initDropZone", _ => true);

        var userApi = new Mock<IApiUser>();
        context.Services.AddMockedService(userApi);
        var identityAuthenticationStateProvider = new Mock<IdentityAuthenticationStateProvider>(userApi.Object);
        context.Services.AddMockedService(identityAuthenticationStateProvider);
    }

    public static void AddCustomDragNDropManager(this TestContext context, IEnumerable<DropItem> dropItemsOnBoard, IEnumerable<DropItem> dropItemsOnBag)
    {
        var dragNDropManager = new Mock<IDragNDropManager>();
        dragNDropManager.Setup(x => x.AllPlayerTiles).Returns(new HashSet<DropItem>());
        dragNDropManager.Setup(x => x.TilesDroppedOnBoard).Returns(dropItemsOnBoard.ToHashSet);
        dragNDropManager.Setup(x => x.TilesDroppedOnBag).Returns(dropItemsOnBag.ToHashSet);
        context.Services.AddMockedService(dragNDropManager);
        context.JSInterop.SetupVoid("mudDragAndDrop.initDropZone", _ => true);
    }

    public static void AddInstantGameApi(this TestContextBase context, InstantGameTestsInput input)
    {
        var instantGameApi = new Mock<IApiInstantGame>();
        instantGameApi.Setup(x => x.JoinInstantGame(input.PlayersNumberNeedToStartGame)).Returns(Task.FromResult(input.InstantGameModel));
        context.Services.AddMockedService(instantGameApi);
    }

    public static void AddGameApi(this TestContextBase context, int gameId, bool gameOver = false)
    {
        var gameApi = new Mock<IApiGame>();
        var player1 = new Player(1, 1, gameId, "player0", 0, 0, 0, Rack.Empty, true, false);
        var player2 = new Player(2, 2, gameId, "player1", 0, 0, 0, Rack.Empty, false, false);
        var players = new List<Player>() { player1, player2 };
        gameApi.Setup(x => x.GetGame(gameId)).Returns(Task.FromResult(new Game(gameId, Board.Empty, players, gameOver)));
        context.Services.AddMockedService(gameApi);
    }

    public static void AddPlayerApi(this TestContextBase context, int gameId)
    {
        var player = new Player(1, 1, gameId, "pseudo", 1, 0, 0, Rack.Empty, true, false);
        var playerApi = new Mock<IApiPlayer>();
        playerApi.Setup(x => x.GetByGameId(gameId)).Returns(Task.FromResult(player));
        context.Services.AddMockedService(playerApi);
    }

    public static void AddSkipTurnApi(this TestContextBase context, SkipTurnReturn skipTurnReturn)
    {
        var skipTurnModel = new SkipTurnModel(skipTurnReturn.GameId);
        var skipTurnReturnTask = Task.FromResult(skipTurnReturn);
        var actionApi = new Mock<IApiAction>();
        actionApi.Setup(x => x.SkipTurn(skipTurnModel)).Returns(skipTurnReturnTask);
        context.Services.AddMockedService(actionApi);
    }

    public static void AddPlayTilesApi(this TestContextBase context, PlayReturn playReturn)
    {
        var playTilesReturnTask = Task.FromResult(playReturn);
        var actionApi = new Mock<IApiAction>();
        actionApi.Setup(x => x.PlayTiles(It.IsAny<List<PlayTileModel>>())).Returns(playTilesReturnTask);
        context.Services.AddMockedService(actionApi);
    }

    public static void AddSwapTilesApi(this TestContextBase context, SwapTilesReturn swapReturn)
    {
        var swapTilesReturnTask = Task.FromResult(swapReturn);
        var actionApi = new Mock<IApiAction>();
        actionApi.Setup(x => x.SwapTiles(It.IsAny<List<SwapTileModel>>())).Returns(swapTilesReturnTask);
        context.Services.AddMockedService(actionApi);
    }

    private static void AddMockedService<T>(this IServiceCollection services, IMock<T> mockedService) where T : class => services.AddSingleton(mockedService.Object);


    private static readonly Player Player1 = new(1, 1, 1, "player1", 0, 0, 0, Rack.Empty, true, false);
    private static readonly Player Player2 = new(2, 2, 1, "player2", 0, 0, 0, Rack.Empty, false, false);
    private static readonly Player Player3 = new(3, 3, 1, "player3", 0, 0, 0, Rack.Empty, false, false);
    private static readonly Player Player4 = new(4, 4, 1, "player4", 0, 0, 0, Rack.Empty, false, false);
}
