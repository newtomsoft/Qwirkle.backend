namespace Qwirkle.WebApi.Client.Blazor.Tests.Arrange;

public class InstantGameTestsInput
{
    public bool ShouldGameBeStarted { get; }
    public int PlayersNumberNeedToStartGame { get; }
    public InstantGameModel InstantGameModel { get; }

    public InstantGameTestsInput(bool shouldGameBeStarted, int playersNumberNeedToStartGame, InstantGameModel instantGameModel)
    {
        PlayersNumberNeedToStartGame = playersNumberNeedToStartGame;
        InstantGameModel = instantGameModel;
        ShouldGameBeStarted = shouldGameBeStarted;
    }
}