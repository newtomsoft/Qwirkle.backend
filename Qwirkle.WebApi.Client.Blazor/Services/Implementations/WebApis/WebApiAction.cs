namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public class WebApiAction : WebApiBase, IApiAction
{
    private const string ControllerName = "Action";

    public WebApiAction(HttpClient httpClient) : base(httpClient) { }

    public async Task<PlayReturn> PlayTiles(List<PlayTileModel> tiles)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/PlayTiles", tiles);
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayReturn>();
    }

    public Task PlayTilesSimulation(List<PlayTileModel> tiles)
    {
        throw new NotImplementedException();
    }

    public async Task<SwapTilesReturn> SwapTiles(List<SwapTileModel> tiles)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/SwapTiles", tiles);
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SwapTilesReturn>();
    }

    public async Task<SkipTurnReturn> SkipTurn(SkipTurnModel skipTurnModel)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/SkipTurn", skipTurnModel);
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SkipTurnReturn>();
    }

    public async Task<ArrangeRackReturn> ArrangeRack(List<ArrangeTileModel> tiles)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/ArrangeRack", tiles);
        if (response.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ArrangeRackReturn>();
    }
}