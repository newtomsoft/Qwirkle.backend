using Qwirkle.Domain.UseCases.Ai;
using System.Reflection;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("Ai")]
public class AiController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly UserManager<UserDao> _userManager;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode _mcts;

    public AiController(BotUseCase botUseCase, UserManager<UserDao> userManager)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
    }

    [HttpGet("BestMoves/{gameId:int}")]
    public ActionResult BestMoves(int gameId)
    {
        _logger.Info($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");

        _mcts = new MonteCarloTreeSearchNode(_botUseCase.GetGame(gameId));
        var playerRoot = _mcts.Game.Players.FirstOrDefault(p => p.IsTurn);
        var playReturns = Expand.ComputeDoableMovesMcts(_mcts.Game.Board, playerRoot, _mcts.Game);
        var random = new Random();
        var playerIndexRoot = _mcts.Game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = Expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);
        var nbSimulation=0;
        var dateOne=DateTime.Now;
        for (var i = 0; i < 8; i++) //todo nommer le i plus explicitement iMctsTry ou un truc du genre ?
        {
            mctsRoot.Children
                .AsParallel()      
                .ForAll(mcts =>
              {
                  var searchPath = new List<MonteCarloTreeSearchNode>();
                  var mctsRollout = mcts;
                  searchPath.Add(mctsRoot);
               
                  while (!mctsRollout.Game.GameOver)
                  {
                      
                      var player = mctsRollout.Game.Players.FirstOrDefault(p => p.IsTurn);
                      var playerIndex = mctsRollout.Game.Players.FindIndex(p => p.IsTurn == true);
                       
                      var currentPlayReturns = Expand.ComputeDoableMovesMcts(mctsRollout.Game.Board, player, mctsRollout.Game);
                       
                    //todo voir si ComputeDoableMoves(Player player, Board board, Coordinates originCoordinates, bool simulation) peut faire l'affaire ou l'adapter


                    if (currentPlayReturns.Count > 0)
                      {
                        
                        var index = random.Next(currentPlayReturns.Count);
                          mctsRollout = Expand.ExpandMctsOne(mctsRollout, currentPlayReturns[index], playerIndex);
                          mctsRollout.Children.First().Game.Players[playerIndex].Points += currentPlayReturns[index].Points;

                          mctsRollout.Children.First().NumberOfVisits++;
                          searchPath.Add(mctsRollout);
                          mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.Children.First().Game, mctsRollout);
                          

                      }
                      else
                      {
                          mctsRollout=Expand.SwapTilesMcts(mctsRollout,playerIndex);
                          
                        player.LastTurnSkipped = true;
                        if (mctsRollout.Game.Bag.Tiles.Count == 0 && mctsRollout.Game.Players.Count(p => p.LastTurnSkipped) == mctsRollout.Game.Players.Count)
                            {
                                
                                mctsRollout.Game.GameOver= true;
                            }
                            else
                                mctsRollout = Expand.SetNextPlayerTurnToPlay(mctsRollout, mctsRollout.Game.Players[playerIndex]);
                          
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

                  }
              });
            
        }
        var dateTwo=DateTime.Now;
        Console.WriteLine((dateTwo-dateOne) + " " + nbSimulation);
        var val = BestChildUCB.BestChildUcb(mctsRoot, 0.1);
        return new ObjectResult(BestChildUCB.BestChildUcb(mctsRoot, 0.1).ParentAction);
    }

 
}