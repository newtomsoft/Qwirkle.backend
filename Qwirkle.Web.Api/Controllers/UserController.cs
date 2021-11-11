

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Route("User")]
public class UserController : ControllerBase
{
    private readonly CoreUseCase _useCase;
    private readonly SignInManager<UserDao> _signInManager;
    private readonly UserManager<UserDao> _userManager;
    private readonly IUserStore<UserDao> _userStore;
    private readonly object _emailStore;
    private readonly IEmailSender _emailSender;

    public UserController(CoreUseCase useCase, SignInManager<UserDao> signInManager, UserManager<UserDao> userManager, IUserStore<UserDao> userStore, IEmailSender emailSender)
    {
        _useCase = useCase;
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }


    [HttpGet("AllUsersIds")]
    public ActionResult<int> GetAllUsersId() => new ObjectResult(_useCase.GetAllUsersId());


    [HttpPost("Register")]
    public async Task<ActionResult<int>> RegisterAsync(LoginViewModel login)
    {
        var userDao = login.ToUserDao();
        await _userStore.SetUserNameAsync(userDao, login.UserName, CancellationToken.None);
        var result = await _userManager.CreateAsync(userDao, login.Password);
        if (!result.Succeeded) return new ObjectResult(0);
        return await LoginAsync(login);
    }

    [HttpPost("Login")]
    public async Task<ActionResult<int>> LoginAsync(LoginViewModel login)
    {
        var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.IsRemember, lockoutOnFailure: false);
        return result.Succeeded ? new ObjectResult(_userManager.GetUserId(User)) : new ObjectResult(0);
    }

    [HttpGet("Logout")]
    public async Task<ActionResult<int>> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return new ObjectResult(true);
    }


}
