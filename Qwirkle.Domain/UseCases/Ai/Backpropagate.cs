namespace Qwirkle.Domain.UseCases.Ai;

public class Backpropagate
{


    public static MonteCarloTreeSearchNode backpropagate(MonteCarloTreeSearchNode mcts, MonteCarloTreeSearchNode node)
    {


        if (node != null)
        {
            node.NumberOfVisits++;
            node.Wins += mcts.Wins;
            node.Looses += mcts.Looses;
            backpropagate(mcts, node.Parent);
        }
        return node;
    }







}