namespace Qwirkle.WebApi.Client.Blazor.Services.Contracts.Apis;

public interface IApiPlayer
{
    Task<Player> GetByGameId(int gameId);
}