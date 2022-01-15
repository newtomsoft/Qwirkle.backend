using System.Reflection;

namespace Qwirkle.Domain.Services;

public class InstantGameService
{
    private readonly INotification _notification;
    private readonly ILogger<InstantGameService> _logger;

    private readonly Dictionary<int, HashSet<int>> _instantGames;

    public InstantGameService(INotification notification, ILogger<InstantGameService> logger)
    {
        _notification = notification;
        _logger = logger;
        _instantGames = new Dictionary<int, HashSet<int>> { { 2, new HashSet<int>(2) }, { 3, new HashSet<int>(3) }, { 4, new HashSet<int>(4) } };
    }

    public HashSet<int> JoinInstantGame(int userId, int playersNumber)
    {
        _logger?.LogInformation($"userId:{userId} {MethodBase.GetCurrentMethod()!.Name} with {_instantGames[playersNumber]}");
        _instantGames[playersNumber].Add(userId);
        var instantGames = new HashSet<int>(_instantGames[playersNumber]);
        if (_instantGames[playersNumber].Count == playersNumber) _instantGames[playersNumber] = new HashSet<int>();
        return instantGames;
    }
}