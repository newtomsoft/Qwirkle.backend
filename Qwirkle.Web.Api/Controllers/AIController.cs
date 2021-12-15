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
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, UserId);
        var random = new Random();
        var playerIndexRoot = _mcts.Game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = Expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);

        for (var i = 0; i < 5; i++) //todo nommer le i plus explicitement iMctsTry ou un truc du genre ?
        {
            mctsRoot.Children.ForEach(mcts =>
            {
                var searchPath = new List<MonteCarloTreeSearchNode>();
                var mctsRollout = mcts;
                searchPath.Add(mctsRoot);
                while (!mctsRollout.Game.GameOver)
                {
                    var player = mctsRollout.Game.Players.FirstOrDefault(p => p.IsTurn);
                    var playerIndex = mctsRollout.Game.Players.FindIndex(p => p.IsTurn);
                    var currentPlayReturns = _botUseCase.ComputeDoableMovesMcts(mctsRollout.Game.Board, player, mctsRollout.Game);
                    //todo voir si ComputeDoableMoves(Player player, Board board, Coordinates originCoordinates, bool simulation) peut faire l'affaire ou l'adapter


                    if (currentPlayReturns.Count > 0)
                    {
                        mctsRollout = Expand.ExpandMcts(mctsRollout, currentPlayReturns, playerIndex);
                        var index = random.Next(currentPlayReturns.Count);
                        var coordinatesrandomAction = new List<PlayReturn> { currentPlayReturns[index] };
                        mctsRollout.Children[index].Game.Players[playerIndex].Points += currentPlayReturns[index].Points;
                        searchPath.Add(mctsRollout);
                        mctsRollout.Children[index].NumberOfVisits++;
                        mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.Children[index].Game, mctsRollout);
                    }
                    else
                    {
                        var removeTiles = mctsRollout.Game.Players[playerIndex].Rack.Tiles;
                        var indexRemoveTile = random.Next(removeTiles.Count);
                        for (var i = 0; i < indexRemoveTile; i++)
                        {
                            var removeTile = removeTiles[random.Next(mctsRollout.Game.Players[playerIndex].Rack.Tiles.Count)];

                            mctsRollout.Game.Players[playerIndex].Rack.Tiles.Remove(removeTile);
                            mctsRollout.Game.Bag.Tiles.Add(new TileOnBag(removeTile.Color, removeTile.Shape));
                        }
                        for (var i = 0; i < indexRemoveTile; i++)
                        {
                            var index = random.Next(mctsRollout.Game.Bag.Tiles.Count);
                            var addTile = mctsRollout.Game.Bag.Tiles[index];
                            mctsRollout.Game.Players[playerIndex].Rack.Tiles.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
                            mctsRollout.Game.Bag.Tiles.Remove(addTile);
                        }

                        mctsRollout = Expand.SetNextPlayerTurnToPlay(mctsRollout, mctsRollout.Game.Players[playerIndex]);
                        searchPath.Add(mctsRollout);
                    }

                }

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
        var val = BestChildUCB.BestChildUcb(mctsRoot, 0.1);
        return new ObjectResult(BestChildUCB.BestChildUcb(mctsRoot, 0.1).ParentAction);
    }
}