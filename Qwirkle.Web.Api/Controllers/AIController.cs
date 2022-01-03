using Qwirkle.Domain.UseCases.Ai;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("[controller]")]
public class AiController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly InfoUseCase _infoUseCase;
    private readonly Expand _expand;
    

    private readonly UserManager<UserDao> _userManager;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode _mcts;

    public AiController(BotUseCase botUseCase, UserManager<UserDao> userManager, Expand expand)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
        _infoUseCase = _botUseCase._infoUseCase;
        _expand=expand;

    }

    [HttpGet("BestMoves/{gameId:int}")]
    public ActionResult BestMoves(int gameId)
    {
        _logger.Info($"userId:{UserId}  with {gameId}");
        _mcts = new MonteCarloTreeSearchNode(_infoUseCase.GetGame(gameId));
        var playerRoot = _mcts.Game.Players.FirstOrDefault(p => p.IsTurn);
        var playReturns = _expand.ComputeDoableMovesMcts(_mcts.Game.Board, playerRoot, _mcts.Game, 0);
        // var playReturnsNew = _botUseCase.GetMostPointsTilesToPlay(playerRoot, _mcts.Game,null);
        if (playReturns.Count == 0) return new ObjectResult("swapRandom");
        if (playReturns.Count > 2) playReturns = playReturns.GetRange(0, 2);

        var random = new Random();
        var playerIndexRoot = _mcts.Game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = _expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);


        var nbSimulation = 0;
        var dateOne = DateTime.Now;
       
            Parallel.ForEach(mctsRoot.Children, mcts =>
              {
                   for (var i = 0; i < 2; i++) //todo nommer le i plus explicitement iMctsTry ou un truc du genre ?
                     {
                  var searchPath = new List<MonteCarloTreeSearchNode>();
                  var mctsRollout = mcts;
                  searchPath.Add(mctsRoot);
                  var randomselect=0;
                 

                  while (!mctsRollout.Game.GameOver)
                  {

                      var player = mctsRollout.Game.Players.FirstOrDefault(p => p.IsTurn);
                      var playerIndex = mctsRollout.Game.Players.FindIndex(p => p.IsTurn == true);
                      List<PlayReturn> currentPlayReturns;
                      
                        currentPlayReturns = _expand.ComputeDoableMovesMcts(mctsRollout.Game.Board, player, mctsRollout.Game,0);
                        
                            
                            

                      if (currentPlayReturns.Count > 0)
                      {

                            

                          mctsRollout = Expand.ExpandMctsOne(mctsRollout, currentPlayReturns[0], playerIndex);
                          mctsRollout.Children.First().Game.Players[playerIndex].Points += currentPlayReturns[0].Points;

                          mctsRollout.Children.First().NumberOfVisits++;
                          searchPath.Add(mctsRollout);
                          mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.Children.First().Game, mctsRollout, currentPlayReturns[0].TilesPlayed);



                      }
                      else
                      {
                          if (mctsRollout.Game.Bag.Tiles.Count != 0)
                          {
                              mctsRollout = Expand.SwapTilesMcts(mctsRollout, playerIndex);
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
                              mctsRollout = Expand.SetNextPlayerTurnToPlay(mctsRollout, mctsRollout.Game.Players[playerIndex]);
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

                  mctsRollout = Backpropagate.BackPropagate(mctsRollout, searchPath[^1]);
                  while (mctsRollout.Parent != null)
                  {

                      mctsRollout = mctsRollout.Parent;

                  }  }
              });

      
        var dateTwo = DateTime.Now;
        Console.WriteLine((dateTwo - dateOne));
        var val = BestChildUCB.BestChildUcb(mctsRoot, 0.1);
        Console.WriteLine("wins:% " + (mctsRoot.Wins * 100 / nbSimulation) + " num sim: " + nbSimulation);
        return new ObjectResult(val.ParentAction);
    }

}