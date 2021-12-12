namespace Qwirkle.Domain.UseCases
{
    public class Backpropagate
    {


      public static MonteCarloTreeSearchNode backpropagate(MonteCarloTreeSearchNode mcts, MonteCarloTreeSearchNode node){

        
             if (node!=null){
                node.number_of_visits++;
                node.wins+=mcts.wins;
                node.looses+=mcts.looses;
                Backpropagate.backpropagate(mcts,node.parent);
         }
         return node;
      }
       
        
          
         
        


    }  
    }
