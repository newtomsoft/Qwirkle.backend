namespace Qwirkle.Domain.UseCases;





public class MonteCarloTreeSearchNode
{
    public Game game { get; set; }
    public MonteCarloTreeSearchNode parent { get; set; }
    public List<TileOnBoard> parent_action { get; set; }
    public List<MonteCarloTreeSearchNode> children { get; set; }

    public int number_of_visits { get; set; }
    public int wins;
    public int looses;

    public MonteCarloTreeSearchNode(Game gameNode, MonteCarloTreeSearchNode parentNode = null, List<TileOnBoard> parent_actionNode = null)
    {
        game = new Game(gameNode);
        parent = parentNode;
        parent_action = parent_actionNode;
        children = new List<MonteCarloTreeSearchNode>();
        number_of_visits = 0;
        wins = 0;
        looses = 0;


    }



}





