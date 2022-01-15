using System.Reflection;

namespace Qwirkle.Domain.Services;

public class InstantGameService
{
    private readonly INotification _notification;
    private readonly ILogger<InstantGameService> _logger;

    private readonly Dictionary<int, HashSet<int>> _instantGamesUsers;

    public InstantGameService(INotification notification, ILogger<InstantGameService> logger)
    {
        _notification = notification;
        _logger = logger;
        _instantGamesUsers = new Dictionary<int, HashSet<int>> { { 2, new HashSet<int>(2) }, { 3, new HashSet<int>(3) }, { 4, new HashSet<int>(4) } };
    }

    public HashSet<int> JoinInstantGame(int userId, int playersNumber)
    {
        _logger?.LogInformation($"userId:{userId} {MethodBase.GetCurrentMethod()!.Name} with {_instantGamesUsers[playersNumber]}");
        _instantGamesUsers[playersNumber].Add(userId);
        var usersIds = new HashSet<int>(_instantGamesUsers[playersNumber]);
        if (_instantGamesUsers[playersNumber].Count == playersNumber) _instantGamesUsers[playersNumber] = new HashSet<int>();
        return usersIds;
    }
}