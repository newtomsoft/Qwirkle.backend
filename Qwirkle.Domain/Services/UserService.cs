namespace Qwirkle.Domain.Services;

public class UserService
{
    private readonly IRepository _repository;
    private readonly IAuthentication _authentication;

    public UserService(IRepository repository, IAuthentication authentication)
    {
        _repository = repository;
        _authentication = authentication;
    }


    public bool IsBot(string userName) => _authentication.IsBot(userName);

    public async Task<bool> Register(User user, string password) => await _authentication.RegisterAsync(user, password);

    public async Task<bool> RegisterGuest() => await _authentication.RegisterGuestAsync();

    [Obsolete]
    public int GetUserId(object user) => _authentication.GetUserId(user);

    public async Task LogOutAsync() => await _authentication.LogoutOutAsync();

    public async Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => await _authentication.LoginAsync(pseudo, password, isRemember);
    
    public bool AddRegisteredOpponent(int userId, string friendName) => _repository.AddRegisteredOpponent(userId, friendName);
}