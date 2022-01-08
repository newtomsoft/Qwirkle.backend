namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserManager<UserDao> _userManager;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");
    public UserController(UserService userService, UserManager<UserDao> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }


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

    [HttpGet("AddBookmarkedOpponent/{friendName}")]
    public ActionResult AddBookmarkedOpponent(string friendName) => new ObjectResult(_userService.AddBookmarkedOpponent(UserId, friendName));

    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
    private static BadRequestObjectResult AlreadyAuthenticated() => new("user already authenticated");
}
