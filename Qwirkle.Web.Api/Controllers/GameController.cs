using Player = Qwirkle.Domain.Entities.Player;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;
    private readonly UserManager<UserDao> _userManager;
    private readonly UserService _userService;
    private readonly BotService _botService;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreService coreService, InfoService infoService, UserManager<UserDao> userManager, UserService userService, BotService botService)
    {
        _logger = logger;
        _coreService = coreService;
        _infoService = infoService;
        _userManager = userManager;
        _userService = userService;
        _botService = botService;
    }


    [HttpPost("New")]
    public ActionResult CreateGame(HashSet<string> usersNames)
    {
        var usersIdsList = new List<int> { UserId };
        usersIdsList.AddRange(usersNames.Select(userName => _infoService.GetUserId(userName)));
        usersIdsList.RemoveAll(id => id == 0);
        var usersIds = new HashSet<int>(usersIdsList);
        _logger?.LogInformation("CreateGame with users {usersIds}", usersIds);
        var players = _coreService.CreateGame(usersIds);
        PlayIfBot(_infoService.GetGame(players[0].GameId));
        return new ObjectResult(players);
    }

    [HttpPost("NewRandom")]
    public ActionResult CreateRandomGame()
    {
        var usersIds = new HashSet<int> { UserId };
        //todo : add user waiting or wait...
        _logger?.LogInformation("CreateGame with {usersIds}", usersIds);
        return new ObjectResult(_coreService.CreateGame(usersIds));
    }


    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId)
    {
        var game = _infoService.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId);
        return new ObjectResult(game);
    }


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => new ObjectResult(_infoService.GetUserGames(UserId));


    private void PlayIfBot(Game game)
    {
        var player = game.Players.First(p => p.IsTurn);
        if (_userService.IsBot(player.Pseudo)) _botService.Play(game, player);
    }
}
