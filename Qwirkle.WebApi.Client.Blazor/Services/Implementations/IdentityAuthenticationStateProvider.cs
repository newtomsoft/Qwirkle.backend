namespace Qwirkle.WebApi.Client.Blazor.Services.Implementations;

public class IdentityAuthenticationStateProvider : AuthenticationStateProvider
{
    private UserInfo? _userInfoCache;
    private readonly IApiUser _apiUser;

    public IdentityAuthenticationStateProvider(IApiUser apiUser)
    {
        _apiUser = apiUser;
    }

    public async Task Login(LoginModel loginModel)
    {
        await _apiUser.Login(loginModel);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Register(RegisterModel registerModel)
    {
        await _apiUser.Register(registerModel);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task RegisterGuest()
    {
        await _apiUser.RegisterGuest();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task Logout()
    {
        await _apiUser.Logout();
        _userInfoCache = null;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            var userInfo = await GetUserInfo();
            if (userInfo is { IsAuthenticated: true })
            {
                var claims = new[] { new Claim(ClaimTypes.Name, userInfo.UserName) }.Concat(userInfo.ExposedClaims.Select(c => new Claim(c.Key, c.Value)));
                identity = new ClaimsIdentity(claims, "Server authentication");
            }
        }
        catch (HttpRequestException exception)
        {
            Console.WriteLine($"Request failed: {exception}");
        }
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private async Task<UserInfo?> GetUserInfo()
    {
        if (_userInfoCache is { IsAuthenticated: true }) return _userInfoCache;
        _userInfoCache = await _apiUser.GetUserInfo();
        return _userInfoCache;
    }
}