namespace Qwirkle.Authentication.Adapters;

public class Authentication : IAuthentication
{
    private readonly UserManager<UserDao> _userManager;
    private readonly SignInManager<UserDao> _signInManager;
    private readonly IUserStore<UserDao> _userStore;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public Authentication(SignInManager<UserDao> signInManager, UserManager<UserDao> userManager, IUserStore<UserDao> userStore, RoleManager<IdentityRole<int>> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _roleManager = roleManager;
    }

    public async Task<bool> RegisterAsync(User user, string password)
    {
        var userDao = user.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, user.Pseudo, CancellationToken.None);
        var result = await _userManager.CreateAsync(userDao, password);
        return result.Succeeded;
    }

    public async Task<bool> RegisterGuestAsync()
    {
        const string guestNamePrefix = "guest";
        const string guestRole = "Guest"; //todo defined in other class

        string guestPseudo;
        do guestPseudo = guestNamePrefix + Guid.NewGuid().ToString("N")[..6];
        while (_userStore.FindByNameAsync(guestPseudo, CancellationToken.None).Result != null);

        var user = new User(guestPseudo, guestPseudo + "@guest");
        var userDao = user.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, user.Pseudo, CancellationToken.None);
        var createGuestResult = await _userManager.CreateAsync(userDao);
        var roleExist = await _roleManager.RoleExistsAsync(guestRole);
        if (!roleExist) await _roleManager.CreateAsync(new IdentityRole<int> { Name = guestRole });
        await _userManager.AddToRoleAsync(userDao, guestRole);
        await _signInManager.SignInAsync(userDao, false);
        return createGuestResult.Succeeded;
    }

    [Obsolete]
    public int GetUserId(object user) => int.Parse(_userManager.GetUserId(user as ClaimsPrincipal) ?? "0");

    public Task LogoutOutAsync() => _signInManager.SignOutAsync();

    public async Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => (await _signInManager.PasswordSignInAsync(pseudo, password, isRemember, false)).Succeeded;

    public bool IsBot(string userName) {
        if (userName == "bot1") return true;
        // Task.Delay(5000).Wait();
        // var result=_userManager.FindByNameAsync(userName).Result;
        // Task.Delay(2000).Wait();
        // var isbot1=_userManager.IsInRoleAsync(result, "Bot").Result;
        return false;
        }
}