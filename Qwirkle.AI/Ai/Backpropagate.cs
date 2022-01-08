namespace Qwirkle.Domain.UseCases.Ai;

public class Backpropagate
{


    public  MonteCarloTreeSearchNode BackPropagate(MonteCarloTreeSearchNode mcts, MonteCarloTreeSearchNode node)
    {


        if (node.Parent != null)
        {
            node.Parent.NumberOfVisits++;
            node.Parent.Wins+= mcts.Wins;
            node.Parent.Looses+= mcts.Looses;
            BackPropagate(mcts, node.Parent);
        }
        return node;
    }







}