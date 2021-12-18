namespace Qwirkle.Authentication.Adapters;

public class Authentication : IAuthentication
{
    private readonly UserManager<UserDao> _userManager;
    private readonly SignInManager<UserDao> _signInManager;
    private readonly IUserStore<UserDao> _userStore;

    public Authentication(SignInManager<UserDao> signInManager, UserManager<UserDao> userManager, IUserStore<UserDao> userStore)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        var userDao = user.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, user.Pseudo, CancellationToken.None);
        var result = await _userManager.CreateAsync(userDao, password);
        return result.Succeeded;
    }

    [Obsolete]
    public int GetUserId(object user) => int.Parse(_userManager.GetUserId(user as ClaimsPrincipal) ?? "0");

    public Task LogoutOutAsync() => _signInManager.SignOutAsync();

    public async Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => (await _signInManager.PasswordSignInAsync(pseudo, password, isRemember, false)).Succeeded;
}