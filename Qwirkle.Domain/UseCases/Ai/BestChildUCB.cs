namespace Qwirkle.Domain.UseCases.Ai;

public class BestChildUCB
{
    public static MonteCarloTreeSearchNode bestChildUCB(MonteCarloTreeSearchNode current, double param)
    {
        MonteCarloTreeSearchNode bestChild = null;
        var best = double.NegativeInfinity;

        foreach (var child in current.Children)
        {
            var UCB1 = (Convert.ToDouble(SetMonteCarloNode.valueWinLoose(child)) / Convert.ToDouble(child.NumberOfVisits)) + param * Math.Sqrt((2.0 * Math.Log(Convert.ToDouble(current.NumberOfVisits))) / Convert.ToDouble(child.NumberOfVisits));

            if (UCB1 > best)
            {
                bestChild = child;
                best = UCB1;
            }
        }

        return bestChild;
    }
    public class SetMonteCarloNode
    {
        public static MonteCarloTreeSearchNode SetWin(MonteCarloTreeSearchNode mcts, int win)
        {

            mcts.Wins = win;
            return mcts;

        }
        public static MonteCarloTreeSearchNode setLoose(MonteCarloTreeSearchNode mcts, int loose)
        {

            mcts.Looses = loose;
            return mcts;

        }
        public static int valueWinLoose(MonteCarloTreeSearchNode mcts)
        {

            return mcts.Wins - mcts.Looses;

        }


    }


}