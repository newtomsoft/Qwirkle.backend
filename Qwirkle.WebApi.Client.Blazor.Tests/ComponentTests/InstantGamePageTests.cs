namespace Qwirkle.WebApi.Client.Blazor.Tests.ComponentTests;

public class InstantGamePageTests
{
    public static TheoryData<InstantGameTestsInput> InstantGameTestsInputs => new()
    {
        new(true, 2, new(true, new[] { "player1", "player2" }, 33)),
        new(false, 2, new(true, new[] { "player1", })),

        new(true, 3, new(true, new[] { "player1", "player2", "player3" }, 69)),
        new(false, 3, new(true, new[] { "player1", "player2" })),
        new(false, 3, new(true, new[] { "player1", })),

        new(true, 4, new(true, new[] { "player1", "player2", "player3", "player4" }, 42)),
        new(false, 4, new(true, new[] { "player1", "player2", "player3" })),
        new(false, 4, new(true, new[] { "player1", "player2" })),
        new(false, 4, new(true, new[] { "player1", })),

        new(false, 4, new(false, Array.Empty<string>())),
    };

    [Theory(DisplayName = "InstantGame Test")]
    [MemberData(nameof(InstantGameTestsInputs))]
    public void InstantGameWaitingPlayersWhenPlayersNumberIsNotEnough(InstantGameTestsInput input)
    {
        using var context = new TestContext();
        context.AddGenericServices();
        context.AddInstantGameApi(input);
        context.Services.AddQwirkleMudServices();
        var instantGamePage = context.RenderComponent<InstantGamePage>();
        var navigationManager = context.Services.GetRequiredService<FakeNavigationManager>();

        instantGamePage.Find($"#btn{input.PlayersNumberNeedToStartGame}Players").Click();
        if (input.ShouldGameBeStarted)
        {
            var newUri = new Uri(navigationManager.Uri);
            newUri.LocalPath.ShouldBe($"{PageName.Game}/{input.InstantGameModel.GameId}");
        }
        else
        {
            var newUri = new Uri(navigationManager.Uri);
            newUri.LocalPath.ShouldNotContain($"{PageName.Game}");
            var waitingPlayerElements = instantGamePage.FindAll(".waitingPlayer").ToList();
            waitingPlayerElements.Count.ShouldBe(input.InstantGameModel.UsersNames.Length);
            for (var i = 0; i < input.InstantGameModel.UsersNames.Length; i++)
            {
                waitingPlayerElements[i].TextContent.ShouldBe(input.InstantGameModel.UsersNames[i]);
                context.SnackBarMessageShouldContain(input.InstantGameModel.UsersNames[i]);
                context.SnackBarMessageShouldContain($"Waiting for {input.PlayersNumberNeedToStartGame - input.InstantGameModel.UsersNames.Length} player(s)", 1);
            }
        }
        if (input.InstantGameModel.IsAdded is false) context.SnackBarMessageShouldBe("You're already waiting for the game");
    }
}