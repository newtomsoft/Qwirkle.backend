using Qwirkle.Domain.Services.Ai;
using Qwirkle.Domain.UseCases.Ai;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for ai
[Route("[controller]")]
public class AiController : ControllerBase
{
    private readonly BotService _botService;
    private readonly InfoService _infoUseCase;
    private readonly Expand _expand;
    private readonly Backpropagate _backpropagate;
    private readonly UserManager<UserDao> _userManager;

    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode _mcts;


    public AiController(BotService botUseCase, InfoService infoService, UserManager<UserDao> userManager, Expand expand, Backpropagate backpropagate)
    {
        _botService = botUseCase;
        _userManager = userManager;
        _infoUseCase = infoService;
        _expand = expand;

        _backpropagate = backpropagate;

    }

    [HttpGet("BestMoves/{gameId:int}")]
    public ActionResult BestMoves(int gameId)
    {

        _mcts = new MonteCarloTreeSearchNode(_infoUseCase.GetGame(gameId));
        var playerRoot = _mcts.Game.Players.FirstOrDefault(p => p.IsTurn);
        var playReturns = _expand.ComputeDoableMovesMcts(_mcts.Game.Board, playerRoot, _mcts.Game, 0);
        // var playReturnsNew = _botUseCase.GetMostPointsTilesToPlay(playerRoot, _mcts.Game,null);
        if (playReturns.Count == 0) return new ObjectResult(null);
        if (playReturns.Count > 2) playReturns = playReturns.GetRange(0, 3);

        var random = new Random();
        var playerIndexRoot = _mcts.Game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = _expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);


        var nbSimulation = 0;
        var dateOne = DateTime.Now;

        mctsRoot.Children.ForEach(mcts =>
          {
              for (var i = 0; i < 35; i++) //todo nommer le i plus explicitement iMctsTry ou un truc du genre ?
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
                          mctsRollout.Children.First().Game.Players[playerIndex].Points += currentPlayReturns[0].Points;

                          mctsRollout.Children.First().NumberOfVisits++;
                          searchPath.Add(mctsRollout);
                          mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.Children.First().Game, mctsRollout, currentPlayReturns[0].TilesPlayed);



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
        return new ObjectResult(val.ParentAction);
    }

}