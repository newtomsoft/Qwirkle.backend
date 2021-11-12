namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("User")]
public class UserController : ControllerBase
{
    private readonly CoreUseCase _useCase;
    private readonly AuthenticationUseCase _authenticationUseCase;
    private readonly SignInManager<UserDao> _signInManager;
    private readonly UserManager<UserDao> _userManager;
    private readonly IUserStore<UserDao> _userStore;

    public UserController(CoreUseCase useCase, AuthenticationUseCase authenticationUseCase, SignInManager<UserDao> signInManager, UserManager<UserDao> userManager, IUserStore<UserDao> userStore)
    {
        _useCase = useCase;
        _authenticationUseCase = authenticationUseCase;
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
    }


    [HttpGet("AllUsersIds")]
    public ActionResult<int> GetAllUsersId() => new ObjectResult(_useCase.GetAllUsersId());


    [HttpPost("Register")]
    public async Task<ActionResult<int>> RegisterAsync(UserViewModel user)
    {
        var result2 = await _authenticationUseCase.Register(user.ToUser(), user.Password);
        return new ObjectResult(result2);


        var userDao = user.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, user.Pseudo, CancellationToken.None);
        var result = await _userManager.CreateAsync(userDao, user.Password);
        return new ObjectResult(result);
    }


    [HttpPost("Login")]
    public async Task<ActionResult<int>> LoginAsync(LoginViewModel login)
    {
        if (User.Identity is { IsAuthenticated: true }) return new ObjectResult(0);
        var result = await _signInManager.PasswordSignInAsync(login.Pseudo, login.Password, login.IsRemember, lockoutOnFailure: false);
        return new ObjectResult(result);
    }


    [HttpGet("WhoAmI")]
    public ActionResult<int> WhoAmI() => new ObjectResult(_userManager.GetUserId(User));


    [HttpGet("Logout")]
    public async Task<ActionResult<int>> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return new ObjectResult(true);
    }
}
