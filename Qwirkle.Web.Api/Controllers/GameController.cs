﻿namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreUseCase _coreUseCase;
    private readonly UserManager<UserDao> _userManager;
    private int _userId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreUseCase coreUseCase, UserManager<UserDao> userManager)
    {
        _logger = logger;
        _coreUseCase = coreUseCase;
        _userManager = userManager;
    }


    [HttpPost("New")]
    public ActionResult CreateGame(List<int> usersIds)
    {
        usersIds.Add(_userId);
        _logger.LogInformation($"CreateGame with {usersIds}");
        return new ObjectResult(_coreUseCase.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId) => new ObjectResult(_coreUseCase.GetGame(gameId));


    [HttpGet("UserGames")]
    public ActionResult GetUserGames() => new ObjectResult(_coreUseCase.GetUserGames(_userId));
}
