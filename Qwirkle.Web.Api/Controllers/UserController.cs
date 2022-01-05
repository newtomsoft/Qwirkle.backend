namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly AuthenticationUseCase _authenticationUseCase;

    public UserController(AuthenticationUseCase authenticationUseCase) => _authenticationUseCase = authenticationUseCase;

    [Authorize]
    [HttpGet("CreateBots")]
    public async Task CreateBotsAsync() => await _authenticationUseCase.CreateBots();


    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<ActionResult> RegisterAsync(UserViewModel userViewModel) => IsAuthenticated() ? AlreadyAuthenticated() : new ObjectResult(await _authenticationUseCase.Register(userViewModel.ToUser(), userViewModel.Password));


    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(LoginViewModel login) => IsAuthenticated() ? AlreadyAuthenticated() : new ObjectResult(await _authenticationUseCase.LoginAsync(login.Pseudo, login.Password, login.IsRemember));

    [AllowAnonymous]
    [HttpGet("RegisterGuest")]
    public async Task<ActionResult> RegisterGuestAsync()
    {
        var result = await _authenticationUseCase.RegisterGuest();
        
        return new ObjectResult(result);
    }

    [Obsolete]
    [HttpGet("WhoAmI")]
    public ActionResult WhoAmI() => new ObjectResult(_authenticationUseCase.GetUserId(User));


    [HttpGet("Logout")]
    public async void LogoutAsync() => await _authenticationUseCase.LogOutAsync();


    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
    private static BadRequestObjectResult AlreadyAuthenticated() => new("user already authenticated");
}
