namespace Qwirkle.Domain.Services.Ai;

public class MonteCarloTreeSearchNode
{
    public Game Game { get; set; }
    public MonteCarloTreeSearchNode Parent { get; set; }
    public List<TileOnBoard> ParentAction { get; set; }
    public List<MonteCarloTreeSearchNode> Children { get; set; }

    public int NumberOfVisits { get; set; }
    public int Wins;
    public int Looses;

    public MonteCarloTreeSearchNode(Game gameNode, MonteCarloTreeSearchNode parentNode = null, List<TileOnBoard> parentActionNode = null)
    {
        Game = new Game(gameNode);
        Parent = parentNode;
        ParentAction = parentActionNode;
        Children = new List<MonteCarloTreeSearchNode>();
        NumberOfVisits = 0;
        Wins = 0;
        Looses = 0;
    }
}