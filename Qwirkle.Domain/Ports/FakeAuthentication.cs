namespace Qwirkle.Domain.Ports;

public class FakeAuthentication : IAuthentication
{
    public int GetUserId(object user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LoginAsync(string pseudo, string password, bool isRemember)
    {
        throw new NotImplementedException();
    }

    public Task LogoutOutAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> RegisterAsync(User user, string password)
    {
        throw new NotImplementedException();
    }
}
