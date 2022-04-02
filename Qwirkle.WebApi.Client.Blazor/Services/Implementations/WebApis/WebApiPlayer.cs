namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public class WebApiPlayer : WebApiBase, IApiPlayer
{
    private const string ControllerName = "Player";

    public WebApiPlayer(HttpClient httpClient) : base(httpClient) { }

    public async Task<Player> GetByGameId(int gameId)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}/{ControllerName}/ByGameId/{gameId}");
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Player>();
    }
}