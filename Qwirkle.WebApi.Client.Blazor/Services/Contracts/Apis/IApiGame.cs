namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Apis;

public interface IApiGame
{
    Task<List<int>> GetUserGamesIds();
    Task<Game> GetGame(int gameId);
    Task<int> CreateGame(OpponentsModel opponentsModel);
    Task<int> CreateSinglePlayerGame();
}