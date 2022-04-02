namespace Qwirkle.WebApi.Client.Blazor.Tests.ComponentTests;

public class GamePageTests
{
    [Fact(DisplayName = "Game Over test")]
    public void GameOverTest()
    {
        const int gameId = 1;

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddGameApi(gameId, true);
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        context.SnackBarMessageShouldBe("Game is over");
    }

    [Theory(DisplayName = "Skip turn test")]
    [InlineData(ReturnCode.Ok, false)]
    [InlineData(ReturnCode.NotPlayerTurn, true)]
    public void SkipTurnTest(ReturnCode skipTurnReturnCode, bool isStackBarMessage)
    {
        const int gameId = 1;

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddGameApi(gameId);
        context.AddSkipTurnApi(new SkipTurnReturn(gameId, skipTurnReturnCode));
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        var gameComponent = context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        gameComponent.Find("#actionResult").Text().ShouldBe(string.Empty);
        gameComponent.Find("#btnSkipTurn").Click();
        gameComponent.Find("#actionResult").Text().ShouldBe(skipTurnReturnCode.ToString());
        if (isStackBarMessage) context.SnackBarMessageShouldBe(skipTurnReturnCode.ToDisplay());
    }


    [Theory(DisplayName = "Play tiles test")]
    [InlineData(ReturnCode.Ok, false)]
    [InlineData(ReturnCode.NotPlayerTurn, true)]
    [InlineData(ReturnCode.TilesDoesntMakeValidRow, true)]
    [InlineData(ReturnCode.TileIsolated, true)]
    [InlineData(ReturnCode.NotFree, true)]
    [InlineData(ReturnCode.NotMostPointsMove, true)]
    [InlineData(ReturnCode.PlayerDoesntHaveThisTile, true)]
    public void PlayTilesTest(ReturnCode playTileReturnCode, bool isStackBarMessage)
    {
        const int gameId = 1;

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddGameApi(gameId);
        context.AddPlayTilesApi(new PlayReturn(gameId, playTileReturnCode, Move.Empty, Rack.Empty));
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        var gameComponent = context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        gameComponent.Find("#actionResult").Text().ShouldBe(string.Empty);
        gameComponent.Find("#btnPlayTiles").Click();
        gameComponent.Find("#actionResult").Text().ShouldBe(playTileReturnCode.ToString());
        if (isStackBarMessage) context.SnackBarMessageShouldBe(playTileReturnCode.ToDisplay());
    }

    [Fact(DisplayName = "Play tiles test When no tile dropped on board")]
    public void PlayTilesTestWhenNoTileDroppedOnBoard()
    {
        const int gameId = 1;
        const string errorMessageExpected = "please move some tiles into board";

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddCustomDragNDropManager(new List<DropItem>(), new List<DropItem>());
        context.AddGameApi(gameId);
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        var gameComponent = context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        gameComponent.Find("#actionResult").Text().ShouldBe(string.Empty);
        gameComponent.Find("#btnPlayTiles").Click();
        gameComponent.Find("#actionResult").Text().ShouldBe(errorMessageExpected);
        context.SnackBarMessageShouldBe(errorMessageExpected);
    }

    [Theory(DisplayName = "Swap tiles test")]
    [InlineData(ReturnCode.Ok, false)]
    [InlineData(ReturnCode.NotPlayerTurn, true)]
    [InlineData(ReturnCode.PlayerDoesntHaveThisTile, true)]
    public void SwapTilesTest(ReturnCode swapTileReturnCode, bool isStackBarMessage)
    {
        const int gameId = 1;

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddGameApi(gameId);
        context.AddSwapTilesApi(new SwapTilesReturn(gameId, swapTileReturnCode, Rack.Empty));
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        var gameComponent = context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        gameComponent.Find("#actionResult").Text().ShouldBe(string.Empty);
        gameComponent.Find("#btnSwapTiles").Click();
        gameComponent.Find("#actionResult").Text().ShouldBe(swapTileReturnCode.ToString());
        if (isStackBarMessage) context.SnackBarMessageShouldBe(swapTileReturnCode.ToDisplay());
    }

    [Fact(DisplayName = "Swap tiles test When no tile dropped on bag")]
    public void SwapTilesTestWhenNoTileDroppedOnBag()
    {
        const int gameId = 1;
        const string errorMessageExpected = "please move some tiles into bag";

        using var context = new TestContext();
        context.AddGenericServices();
        context.AddCustomDragNDropManager(new List<DropItem>(), new List<DropItem>());
        context.AddGameApi(gameId);
        context.AddPlayerApi(gameId);
        context.Services.AddQwirkleMudServices();

        var gameComponent = context.RenderComponent<GamePage>(parameters => parameters.Add(p => p.GameId, gameId));
        gameComponent.Find("#actionResult").Text().ShouldBe(string.Empty);
        gameComponent.Find("#btnSwapTiles").Click();
        gameComponent.Find("#actionResult").Text().ShouldBe(errorMessageExpected);
        context.SnackBarMessageShouldBe(errorMessageExpected);
    }
}