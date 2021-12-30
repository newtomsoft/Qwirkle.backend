namespace Qwirkle.Domain.UseCases.Ai;

public class Backpropagate
{


    public static MonteCarloTreeSearchNode BackPropagate(MonteCarloTreeSearchNode mcts, MonteCarloTreeSearchNode node)
    {


        if (node.Parent != null)
        {
            node.Parent.NumberOfVisits++;
            if (mcts.Wins==1) node.Parent.Looses += 1;
            if (mcts.Looses==1) node.Parent.Wins += 1;
            BackPropagate(mcts, node.Parent);
        }
        return node;
    }







}