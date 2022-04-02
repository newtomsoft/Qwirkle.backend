namespace Qwirkle.Domain.Services;

public class UserService
{
    private readonly IRepository _repository;
    private readonly IAuthentication _authentication;

    public const string RoleAdminName = "Admin";
    public const string RoleGuestName = "Guest";
    public const string RoleUserName = "User";
    public const string RoleBotName = "Bot";
    public const int PassWordMinLength = 6;

    public UserService(IRepository repository, IAuthentication authentication)
    {
        _repository = repository;
        _authentication = authentication;
    }

    public bool IsBot(int userId) => _authentication.IsBot(userId);

    public async Task<bool> Register(User user, string password, bool isSignInPersistent) => await _authentication.RegisterAsync(user, password, isSignInPersistent);

    public async Task<bool> RegisterGuest() => await _authentication.RegisterGuestAsync();

    public async Task LogOutAsync() => await _authentication.LogoutOutAsync();

    public async Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => await _authentication.LoginAsync(pseudo, password, isRemember);

    public bool AddBookmarkedOpponent(int userId, string friendName) => _repository.AddBookmarkedOpponent(userId, friendName);
    public bool RemoveBookmarkedOpponent(int userId, string friendName) => _repository.RemoveBookmarkedOpponent(userId, friendName);
    public HashSet<string> GetBookmarkedOpponentsNames(int userId) => _repository.GetBookmarkedOpponentsNames(userId);
}