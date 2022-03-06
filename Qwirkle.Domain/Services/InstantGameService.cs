namespace Qwirkle.Domain.Services;

public class InstantGameService
{
    private readonly ILogger<InstantGameService> _logger;

    private readonly Dictionary<int, HashSet<int>> _instantGamesUsers;
    private static readonly object LockObject = new();

    public InstantGameService(ILogger<InstantGameService> logger)
    {
        _logger = logger;
        _instantGamesUsers = new Dictionary<int, HashSet<int>> { { 2, new HashSet<int>(2) }, { 3, new HashSet<int>(3) }, { 4, new HashSet<int>(4) } };
    }

    public HashSet<int> JoinInstantGame(int userId, int playersNumber)
    {
        lock (LockObject)
        {
            _logger?.LogInformation($"userId:{userId} {MethodBase.GetCurrentMethod()!.Name} with {_instantGamesUsers[playersNumber]}");
            _instantGamesUsers[playersNumber].Add(userId);
            var usersIds = new HashSet<int>(_instantGamesUsers[playersNumber]);
            if (_instantGamesUsers[playersNumber].Count == playersNumber) _instantGamesUsers[playersNumber] = new HashSet<int>();
            return usersIds;
        }
    }
}