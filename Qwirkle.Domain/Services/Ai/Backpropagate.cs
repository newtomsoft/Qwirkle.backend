namespace Qwirkle.Domain.Services.Ai;

public class Backpropagate
{


    public static MonteCarloTreeSearchNode BackPropagate(MonteCarloTreeSearchNode mcts, MonteCarloTreeSearchNode node)
    {


        if (node != null)
        {
            node.NumberOfVisits++;
            node.Wins += mcts.Wins;
            node.Looses += mcts.Looses;
            BackPropagate(mcts, node.Parent);
        }
        return node;
    }







}