namespace Qwirkle.Domain.Ports;

public class FakeAuthentication : IAuthentication
{
    public Task<bool> RegisterGuestAsync() => throw new NotSupportedException();
    public int GetUserId(object user) => throw new NotSupportedException();
    public Task<bool> LoginAsync(string pseudo, string password, bool isRemember) => throw new NotSupportedException();
    public bool IsBot(string userName) => false;
    public Task LogoutOutAsync() => throw new NotSupportedException();
    public Task<bool> RegisterAsync(User user, string password) => throw new NotSupportedException();
}
