namespace Qwirkle.Domain.Ports;

public interface IAuthentication
{
    Task CreateBotsAsync();
    Task<bool> RegisterAsync(User user, string password);
    Task<bool> RegisterGuestAsync();
    [Obsolete]
    int GetUserId(object user);
    Task LogoutOutAsync();
    Task<bool> LoginAsync(string pseudo, string password, bool isRemember);
}