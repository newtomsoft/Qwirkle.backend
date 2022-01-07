using Qwirkle.Domain.Services.Ai;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("[controller]")]
public class AiController : ControllerBase
{
    private readonly BotService _botService;
    private readonly UserManager<UserDao> _userManager;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode _mcts;

    public AiController(BotService botService, UserManager<UserDao> userManager)
    {
        _botService = botService;
        _userManager = userManager;
    }


}