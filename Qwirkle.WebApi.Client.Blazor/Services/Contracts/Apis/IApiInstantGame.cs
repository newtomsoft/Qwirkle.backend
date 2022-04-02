namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Apis;

public interface IApiInstantGame
{
    public Task<InstantGameModel> JoinInstantGame(int playersNumber);
}