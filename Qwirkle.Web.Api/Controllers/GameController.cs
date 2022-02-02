using Player = Qwirkle.Domain.Entities.Player;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize]
[Route("Game")]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly CoreService _coreService;
    private readonly InfoService _infoService;
    private readonly UserManager<UserDao> _userManager;
    private readonly UserService _userService;
    private readonly BotService _botService;
    private readonly INotification _notification;
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    public GameController(ILogger<GameController> logger, CoreService coreService, InfoService infoService, UserManager<UserDao> userManager, UserService userService, BotService botService, INotification notification)
    {
        _logger = logger;
        _coreService = coreService;
        _infoService = infoService;
        _userManager = userManager;
        _userService = userService;
        _botService = botService;
        _notification = notification;
    }


    [HttpPost("New")]
    public ActionResult CreateGame(HashSet<string> usersNames)
    {
        var usersIdsList = new List<int> { UserId };
        usersIdsList.AddRange(usersNames.Select(userName => _infoService.GetUserId(userName)));
        usersIdsList.RemoveAll(id => id == 0);
        var usersIds = new HashSet<int>(usersIdsList);
        var players = CreateGameWithUsersIds(usersIds);
        return new ObjectResult(players);
    }

    [HttpGet("{gameId:int}")]
    public ActionResult GetGame(int gameId)
    {
        var game = _infoService.GetGameWithTilesOnlyForAuthenticatedUser(gameId, UserId);
        return new ObjectResult(game);
    }


    [HttpGet("UserGamesIds")]
    public ActionResult GetUserGamesIds() => new ObjectResult(_infoService.GetUserGames(UserId));


    public ActionResult CreateInstantGame(string serializedUsersIds)
    {
        var usersIds = JsonConvert.DeserializeObject<HashSet<int>>(serializedUsersIds);
        var players = CreateGameWithUsersIds(usersIds);
        if (players.Count == 0) return new BadRequestObjectResult("user not in the game");
        _notification.SendInstantGameStarted(usersIds.Count, players.First().GameId);
        return new ObjectResult(players);
    }

    private List<Player> CreateGameWithUsersIds(HashSet<int> usersIds)
    {
        if (!usersIds.Contains(UserId)) return new List<Player>();
        _logger?.LogInformation("CreateGame with users {usersIds}", usersIds);
        var players = _coreService.CreateGame(usersIds).Players;
        PlayIfBot(_infoService.GetGame(players[0].GameId));
        return players;
    }

    private void PlayIfBot(Game game)
    {
        var player = game.Players.First(p => p.IsTurn);
        if (_userService.IsBot(player.UserId)) _botService.Play(game, player);
    }
}
