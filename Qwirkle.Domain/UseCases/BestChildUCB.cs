namespace Qwirkle.Domain.UseCases{
public class BestChildUCB{
public  static MonteCarloTreeSearchNode bestChildUCB(MonteCarloTreeSearchNode current, double param)
{
    MonteCarloTreeSearchNode bestChild = null;
    double best = double.NegativeInfinity;

    foreach (MonteCarloTreeSearchNode child in current.children)
    {
        double UCB1 = (Convert.ToDouble(SetMonteCarloNode.valueWinLoose(child)) / Convert.ToDouble(child.number_of_visits)) + param * Math.Sqrt((2.0 * Math.Log(Convert.ToDouble(current.number_of_visits))) / Convert.ToDouble(child.number_of_visits));

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
        public static MonteCarloTreeSearchNode SetWin(MonteCarloTreeSearchNode mcts, int win){

                mcts.wins=win;
            return mcts;

        }
        public static MonteCarloTreeSearchNode setLoose(MonteCarloTreeSearchNode mcts, int loose){

                mcts.looses=loose;
            return mcts;

        }
        public static int valueWinLoose(MonteCarloTreeSearchNode mcts){

            return mcts.wins-mcts.looses;

        }
        
       
    }


}}