namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService) => _userService = userService;


    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<ActionResult> RegisterAsync(UserViewModel userViewModel) => IsAuthenticated() ? AlreadyAuthenticated() : new ObjectResult(await _userService.Register(userViewModel.ToUser(), userViewModel.Password));


    [AllowAnonymous]
    [HttpGet("RegisterGuest")]
    public async Task<ActionResult> RegisterGuestAsync() => new ObjectResult(await _userService.RegisterGuest());


    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(LoginViewModel login) => IsAuthenticated() ? AlreadyAuthenticated() : new ObjectResult(await _userService.LoginAsync(login.Pseudo, login.Password, login.IsRemember));


    [HttpGet("Logout")]
    public async void LogoutAsync() => await _userService.LogOutAsync();

    [Obsolete]
    [HttpGet("WhoAmI")]
    public ActionResult WhoAmI() => new ObjectResult(_userService.GetUserId(User));

    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
    private static BadRequestObjectResult AlreadyAuthenticated() => new("user already authenticated");
}
