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

    public async Task CreateBotsAsync()
    {
        //TODO à appeler une seule fois à la création de la base
        const string botRole = "Bot";
        var isBotRoleExist = await _roleManager.RoleExistsAsync(botRole);
        if (!isBotRoleExist) await _roleManager.CreateAsync(new IdentityRole<int> { Name = botRole });
        for (var i = 1; i <= 4; i++)
        {
            var botPseudo = "bot" + i;
            if (_userStore.FindByNameAsync(botPseudo, CancellationToken.None).Result != null) continue;

            var botUser = new User { Pseudo = botPseudo, Email = botPseudo + "@bot" };
            var botUserDao = botUser.ToUserDao();
            await _userStore.SetUserNameAsync(botUserDao, botUser.Pseudo, CancellationToken.None);
            await _userManager.CreateAsync(botUserDao);
            await _userManager.AddToRoleAsync(botUserDao, botRole);
        }

        //TODO mettre ailleurs à la création de la base
        const string adminRole = "Admin";
        var isAdminRoleExist = await _roleManager.RoleExistsAsync(adminRole);
        if (!isAdminRoleExist) await _roleManager.CreateAsync(new IdentityRole<int> { Name = adminRole });

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
        const string guestRole = "Guest";

        string guestPseudo;
        do guestPseudo = guestNamePrefix + Guid.NewGuid().ToString("N")[..6];
        while (_userStore.FindByNameAsync(guestPseudo, CancellationToken.None).Result != null);

        var user = new User { Pseudo = guestPseudo, Email = guestPseudo + "@guest" };
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
}