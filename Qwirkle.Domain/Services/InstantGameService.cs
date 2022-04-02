namespace Qwirkle.Domain.Services;

public class InstantGameService
{
    private readonly ILogger<InstantGameService> _logger;

    private readonly Dictionary<int, HashSet<string>> _instantGamesUsers;
    private static readonly object LockObject = new();

    public InstantGameService(ILogger<InstantGameService> logger)
    {
        _logger = logger;
        _instantGamesUsers = new Dictionary<int, HashSet<string>> { { 2, new HashSet<string>(2) }, { 3, new HashSet<string>(3) }, { 4, new HashSet<string>(4) } };
    }

    public InstantGameResult JoinInstantGame(string userName, int playersNumber)
    {
        lock (LockObject)
        {
            _logger?.LogInformation($"userName:{userName} {MethodBase.GetCurrentMethod()!.Name} with {_instantGamesUsers[playersNumber]}");
            var isAdded = _instantGamesUsers[playersNumber].Add(userName);
            var usersNames = new HashSet<string>(_instantGamesUsers[playersNumber]);
            if (_instantGamesUsers[playersNumber].Count == playersNumber) _instantGamesUsers[playersNumber] = new HashSet<string>();
            return new() { IsAdded = isAdded, UsersNames = usersNames };
        }
    }
}