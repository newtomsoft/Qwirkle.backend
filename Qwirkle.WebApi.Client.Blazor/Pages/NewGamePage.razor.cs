namespace Qwirkle.WebApi.Client.Blazor.Pages;

public partial class NewGamePage : ComponentBase
{
    [Inject] private IApiGame ApiGame { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private OpponentsModel OpponentsModel { get; } = new();
    private string Error { get; set; } = string.Empty;


    private async Task CreateGameWithBot(int botsNumber)
    {
        if (botsNumber is < 1 or > 3) throw new ArgumentException("invalid bots number");

        const string bot1 = "bot1";
        var bot2 = botsNumber >= 2 ? "bot2" : null;
        var bot3 = botsNumber == 3 ? "bot3" : null;
        var gameId = await ApiGame.CreateGame(new OpponentsModel(bot1, bot2, bot3));
        NavigationManager.NavigateTo($"{PageName.Game}/{gameId}");
    }

    private async Task CreateGameWithPlayers()
    {
        var gameId = await ApiGame.CreateGame(OpponentsModel);
        NavigationManager.NavigateTo($"{PageName.Game}/{gameId}");
    }
}
