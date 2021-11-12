using System.Threading;
using System.Threading.Tasks;

namespace Qwirkle.Core.UseCases;

public class AuthenticationUseCase
{
    private readonly IAuthentication _authentication;

    public Game Game { get; set; }

    public AuthenticationUseCase(IAuthentication authentication)
    {
        _authentication = authentication;
    }

    public async Task<bool> Register(User user, string password)
    {
        return await _authentication.RegisterAsync(user, password);
    }
}