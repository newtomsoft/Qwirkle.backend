namespace Qwirkle.WebApi.Shared;

[Serializable]
public class InstantGameModel
{
    public bool IsAdded { get; init; }
    public string[] UsersNames { get; init; }
    public int GameId { get; init; }

    public InstantGameModel() { }

    public InstantGameModel(bool isAdded, string[] usersNames, int gameId)
    {
        if (gameId <= 0) throw new ArgumentException("gameId Must be > 0 if present");
        IsAdded = isAdded;
        UsersNames = usersNames;
        GameId = gameId;
    }

    public InstantGameModel(bool isAdded, string[] usersNames)
    {
        IsAdded = isAdded;
        UsersNames = usersNames;
    }
}
