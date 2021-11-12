namespace Qwirkle.Core.Ports;

public interface IAuthentication
{
    Task<bool> RegisterAsync(User user, string password);
    int GetUserId(object user);
    Task LogoutOutAsync();
    Task<bool> LoginAsync(string pseudo, string password, bool isRemember);
}