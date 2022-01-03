using Qwirkle.Domain.UseCases.Ai;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("[controller]")]
public class AiController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly UserManager<UserDao> _userManager;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode _mcts;

    public AiController(BotUseCase botUseCase, UserManager<UserDao> userManager)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
    }


}