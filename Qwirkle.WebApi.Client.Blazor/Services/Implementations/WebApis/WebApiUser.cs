namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations.WebApis;

public class WebApiUser : WebApiBase, IApiUser
{
    private const string ControllerName = "User";

    public WebApiUser(HttpClient httpClient) : base(httpClient) { }

    public async Task Login(LoginModel loginModel)
    {
        var result = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/Login", loginModel);
        if (result.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
        result.EnsureSuccessStatusCode();
    }

    public async Task Logout()
    {
        var result = await _httpClient.PostAsync($"{ApiPrefix}/{ControllerName}/Logout", null);
        result.EnsureSuccessStatusCode();
    }

    public async Task Register(RegisterModel registerModel)
    {
        var result = await _httpClient.PostAsJsonAsync($"{ApiPrefix}/{ControllerName}/Register", registerModel);
        if (result.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
        result.EnsureSuccessStatusCode();
    }

    public async Task RegisterGuest()
    {
        var result = await _httpClient.GetAsync($"{ApiPrefix}/{ControllerName}/RegisterGuest");
        if (result.StatusCode == HttpStatusCode.BadRequest) throw new Exception(await result.Content.ReadAsStringAsync());
        result.EnsureSuccessStatusCode();
    }

    public async Task<UserInfo> GetUserInfo() => await _httpClient.GetFromJsonAsync<UserInfo>($"{ApiPrefix}/{ControllerName}/Info");
}