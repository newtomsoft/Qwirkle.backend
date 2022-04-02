namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public class WebApiGame : WebApiBase, IApiGame
{
    private const string ControllerName = "Game";

    public WebApiGame(HttpClient httpClient) : base(httpClient) { }

    public async Task<int> CreateGame(OpponentsModel opponentsModel)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/New", opponentsModel);
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<int> CreateSinglePlayerGame()
    {
        var opponent = new OpponentsModel();
        return await CreateGame(opponent);
    }

    public async Task<List<int>> GetUserGamesIds()
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}/{ControllerName}/UserGamesIds");
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<int>>();
    }

    public async Task<Game> GetGame(int gameId)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}/{ControllerName}/{gameId}");
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Game>();
    }

}