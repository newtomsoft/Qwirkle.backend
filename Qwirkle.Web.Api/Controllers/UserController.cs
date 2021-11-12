namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("User")]
public class UserController : ControllerBase
{
    private readonly AuthenticationUseCase _authenticationUseCase;

    public UserController(AuthenticationUseCase authenticationUseCase)
    {
        _authenticationUseCase = authenticationUseCase;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<int>> RegisterAsync(UserViewModel userViewModel) => new ObjectResult(await _authenticationUseCase.Register(userViewModel.ToUser(), userViewModel.Password));


    [HttpPost("Login")]
    public async Task<ActionResult<int>> LoginAsync(LoginViewModel login) => IsAuthenticated() ? new ObjectResult(0) : new ObjectResult(await _authenticationUseCase.LoginAsync(login.Pseudo, login.Password, login.IsRemember));

    [Authorize]
    [HttpGet("WhoAmI")]
    public ActionResult<int> WhoAmI() => new ObjectResult(_authenticationUseCase.GetUserId(User));

    [Authorize]
    [HttpGet("Logout")]
    public async void LogoutAsync() => await _authenticationUseCase.LogOutAsync();


    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
}
