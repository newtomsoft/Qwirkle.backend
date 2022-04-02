namespace Qwirkle.WebApi.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/User")]
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
    public async Task<ActionResult> RegisterAsync(RegisterModel registerModel) => IsAuthenticated() ? AlreadyAuthenticated() : new ObjectResult(await _userService.Register(registerModel.ToUser(), registerModel.Password, registerModel.SignInPersistent));


    [AllowAnonymous]
    [HttpGet("RegisterGuest")]
    public async Task<ActionResult> RegisterGuestAsync() => new ObjectResult(await _userService.RegisterGuest());


    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult> LoginAsync(LoginModel login)
    {
        if (IsAuthenticated()) return AlreadyAuthenticated();
        var isGoodLogin = await _userService.LoginAsync(login.UserName, login.Password, login.RememberMe);
        return isGoodLogin ? Ok() : BadRequest("User or login invalid");
    }


    [HttpPost("Logout")]
    public async Task LogoutAsync() => await _userService.LogOutAsync();


    [AllowAnonymous]
    [HttpGet("Info")]
    public UserInfo UserInfo()
    {
        return new UserInfo
        {
            IsAuthenticated = User.Identity.IsAuthenticated,
            UserName = User.Identity.Name,
            ExposedClaims = User.Claims
                .ToDictionary(c => c.Type, c => c.Value)
        };
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("IsAdmin")]
    public ActionResult IsAdmin() => StatusCode(StatusCodes.Status200OK);


    [HttpGet("AddBookmarkedOpponent/{friendName}")]
    public ActionResult AddBookmarkedOpponent(string friendName) => new ObjectResult(_userService.AddBookmarkedOpponent(UserId, friendName));


    [HttpDelete("RemoveBookmarkedOpponent/{friendName}")]
    public ActionResult RemoveBookmarkedOpponent(string friendName) => new ObjectResult(_userService.RemoveBookmarkedOpponent(UserId, friendName));


    [HttpGet("BookmarkedOpponents")]
    public ActionResult GetBookmarkedOpponentsNames() => new ObjectResult(_userService.GetBookmarkedOpponentsNames(UserId));

    private bool IsAuthenticated() => User.Identity is { IsAuthenticated: true };
    private static BadRequestObjectResult AlreadyAuthenticated() => new("user already authenticated");
}
