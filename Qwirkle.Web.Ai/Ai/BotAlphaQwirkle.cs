using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;



namespace Qwirkle.Web.Ai;



public class BotAlphaQwirkle
{
    private readonly InfoService _infoService;
    private readonly Expand _expand;
    private readonly Backpropagate _backpropagate;
    private readonly BotService _botService;

    public BotAlphaQwirkle(InfoService infoService, BotService botService)
    {
        _infoService = infoService;
        _botService = botService;
        _expand = new Expand();
        _backpropagate = new Backpropagate();
    }

    public HashSet<TileOnBoard> AlphaQwirkle(int gameId)
    {
        var _mcts = new MonteCarloTreeSearchNode(_infoService.GetGame(gameId));
        var test = new ConvertGameToAlpha();
         var playerRoot = _mcts.Game.Players.FirstOrDefault(p => p.IsTurn);
        var tInput = test.GetTensorBoardInput(_mcts,playerRoot.Rack);
        var cnn = new ConnectNet();
        var essai = cnn.forwardTest(tInput);
        var optimizer = torch.optim.Adam(cnn.parameters(), 0.0000001);
        var alphaLoss = new AlphaLoss();
       
        var playReturns = _expand.ComputeDoableMovesMcts(_mcts.Game.Board, playerRoot, _mcts.Game, 0);
        if (playReturns.Count == 0) return null;
        if (playReturns.Count > 10) playReturns = playReturns.GetRange(0, 10);
        var playerIndexRoot = _mcts.Game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = _expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);


        var nbSimulation = 0;
        var dateOne = DateTime.Now;

        mctsRoot.Children.ForEach(mcts =>
          {
              for (var i = 0; i < 10; i++) //todo nommer le i plus explicitement iMctsTry ou un truc du genre ?
              {
                  var searchPath = new List<MonteCarloTreeSearchNode>();
                  var mctsRollout = mcts;
                  searchPath.Add(mctsRoot);
                  var randomselect = 0;


                  while (!mctsRollout.Game.GameOver)
                  {

                      var player = mctsRollout.Game.Players.FirstOrDefault(p => p.IsTurn);
                      var playerIndex = mctsRollout.Game.Players.FindIndex(p => p.IsTurn == true);
                      List<PlayReturn> currentPlayReturns;

                      currentPlayReturns = _expand.ComputeDoableMovesMcts(mctsRollout.Game.Board, player, mctsRollout.Game, randomselect);
                      randomselect++;



                      if (currentPlayReturns.Count > 0)
                      {



                          mctsRollout = _expand.ExpandMctsOne(mctsRollout, currentPlayReturns[0], playerIndex);
                          mctsRollout.Children.First().Game.Players[playerIndex].Points += currentPlayReturns[0].Move.Points;

                          mctsRollout.Children.First().NumberOfVisits++;
                          searchPath.Add(mctsRollout);
                          mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.Children.First().Game, mctsRollout, currentPlayReturns[0].Move.Tiles.ToHashSet());



                      }
                      else
                      {
                          if (mctsRollout.Game.Bag.Tiles.Count != 0)
                          {
                              mctsRollout = _expand.SwapTilesMcts(mctsRollout, playerIndex);
                          }
                          else
                          {
                              player.LastTurnSkipped = true;
                          }
                          if (mctsRollout.Game.Players.Count(p => p.LastTurnSkipped) == mctsRollout.Game.Players.Count)
                          {

                              mctsRollout.Game = new Game(mctsRollout.Game.Id, mctsRollout.Game.Board, mctsRollout.Game.Players, true);
                          }
                          else
                          {
                              mctsRollout = _expand.SetNextPlayerTurnToPlay(mctsRollout, mctsRollout.Game.Players[playerIndex]);
                          }
                          searchPath.Add(mctsRollout);
                      }


                  }
                  nbSimulation++;
                  var score = mctsRollout.Game.Players.Select(p => p.Points).ToList().Max();
                  if (score == mctsRollout.Game.Players[playerIndexRoot].Points)
                  {
                      mctsRollout.Wins++;
                  }
                  else
                  {
                      mctsRollout.Looses++;
                  }

                  mctsRollout = _backpropagate.BackPropagate(mctsRollout, searchPath[^1]);
                  while (mctsRollout.Parent != null)
                  {

                      mctsRollout = mctsRollout.Parent;

                  }
              }
          });


        var dateTwo = DateTime.Now;

        var val = BestChildUCB.BestChildUcb(mctsRoot, 0.1);
        Console.WriteLine("time:" + (dateTwo) + " wins:% " + (mctsRoot.Wins * 100 / nbSimulation) + " num sim: " + nbSimulation);
        return val.ParentAction;
    }
}
