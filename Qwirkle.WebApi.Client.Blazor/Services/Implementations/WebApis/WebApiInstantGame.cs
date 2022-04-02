namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public class WebApiInstantGame : WebApiBase, IApiInstantGame
{
    private const string ControllerName = "InstantGame";

    public WebApiInstantGame(HttpClient httpClient) : base(httpClient) { }

    public async Task<InstantGameModel> JoinInstantGame(int playersNumber)
    {
        var response = await _httpClient.GetAsync($"{ApiPrefix}/{ControllerName}/Join/{playersNumber}");
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<InstantGameModel>();
    }
}