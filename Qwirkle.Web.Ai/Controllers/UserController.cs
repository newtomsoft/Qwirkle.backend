namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("User")]
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
    public async Task<ActionResult> RegisterAsync(UserViewModel userViewModel) => IsAuthenticated() ? BadRequest("user already authenticated") : Ok(await _userService.Register(userViewModel.ToUser(), userViewModel.Password));


    [AllowAnonymous]
    [HttpGet("RegisterGuest")]
    public async Task<ActionResult> RegisterGuestAsync() => Ok(await _userService.RegisterGuest());


    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(LoginViewModel login) => IsAuthenticated() ? BadRequest("user already authenticated") : Ok(await _userService.LoginAsync(login.Pseudo, login.Password, login.IsRemember));


    [HttpGet("Logout")]
    public async Task LogoutAsync() => await _userService.LogOutAsync();


    [Authorize(Roles = "Admin")]
    [HttpGet("IsAdmin")]
    public ActionResult IsAdmin() => Ok();


    [HttpGet("AddBookmarkedOpponent/{friendName}")]
    public ActionResult AddBookmarkedOpponent(string friendName) => Ok(_userService.AddBookmarkedOpponent(UserId, friendName));


    [HttpDelete("RemoveBookmarkedOpponent/{friendName}")]
    public ActionResult RemoveBookmarkedOpponent(string friendName) => Ok(_userService.RemoveBookmarkedOpponent(UserId, friendName));


    [HttpGet("BookmarkedOpponents")]
    public ActionResult GetBookmarkedOpponentsNames() => Ok(_userService.GetBookmarkedOpponentsNames(UserId));


    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
}
