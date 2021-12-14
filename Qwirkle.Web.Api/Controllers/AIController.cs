using System.Reflection;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
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
        var playerIndexRoot = _mcts.game.Players.FindIndex(player => player.IsTurn);
        var mctsRoot = Expand.ExpandMcts(_mcts, playReturns, playerIndexRoot);

        for (var i = 0; i < 5; i++)
        {
            mctsRoot.children.ForEach(mcts =>
            {
                var searchPath = new List<MonteCarloTreeSearchNode>();
                var mctsRollout = mcts;
                searchPath.Add(mctsRoot);
                while (!mctsRollout.game.GameOver)
                {
                    var player = mctsRollout.game.Players.FirstOrDefault(p => p.IsTurn);
                    var playerIndex = mctsRollout.game.Players.FindIndex(p => p.IsTurn);
                    var currentPlayReturns = _botUseCase.ComputeDoableMovesMcts(mctsRollout.game.Board, player, mctsRollout.game);
                    if (currentPlayReturns.Count > 0)
                    {
                        mctsRollout = Expand.ExpandMcts(mctsRollout, currentPlayReturns, playerIndex);
                        var index = random.Next(currentPlayReturns.Count);
                        var coordinatesrandomAction = new List<PlayReturn> { currentPlayReturns[index] };
                        mctsRollout.children[index].game.Players[playerIndex].Points += currentPlayReturns[index].Points;
                        searchPath.Add(mctsRollout);
                        mctsRollout.children[index].number_of_visits++;
                        mctsRollout = new MonteCarloTreeSearchNode(mctsRollout.children[index].game, mctsRollout);
                    }
                    else
                    {
                        var removeTiles = mctsRollout.game.Players[playerIndex].Rack.Tiles;
                        var indexRemoveTile = random.Next(removeTiles.Count);
                        for (var i = 0; i < indexRemoveTile; i++)
                        {
                            var removeTile = removeTiles[random.Next(mctsRollout.game.Players[playerIndex].Rack.Tiles.Count)];

                            mctsRollout.game.Players[playerIndex].Rack.Tiles.Remove(removeTile);
                            mctsRollout.game.Bag.Tiles.Add(new TileOnBag(removeTile.Color, removeTile.Shape));
                        }
                        for (var i = 0; i < indexRemoveTile; i++)
                        {
                            var index = random.Next(mctsRollout.game.Bag.Tiles.Count);
                            var addTile = mctsRollout.game.Bag.Tiles[index];
                            mctsRollout.game.Players[playerIndex].Rack.Tiles.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
                            mctsRollout.game.Bag.Tiles.Remove(addTile);
                        }

                        mctsRollout = Expand.SetNextPlayerTurnToPlay(mctsRollout, mctsRollout.game.Players[playerIndex]);
                        searchPath.Add(mctsRollout);
                    }

                }

                var score = mctsRollout.game.Players.Select(p => p.Points).ToList().Max();
                if (score == mctsRollout.game.Players[playerIndexRoot].Points)
                {
                    mctsRollout.wins++;
                }
                else
                {
                    mctsRollout.looses++;
                }

                mctsRollout = Backpropagate.backpropagate(mctsRollout, searchPath[^1]);
                while (mctsRollout.parent != null)
                {

                    mctsRollout = mctsRollout.parent;

                }

            });


        }
        var val = BestChildUCB.bestChildUCB(mctsRoot, 0.1);
        return new ObjectResult(BestChildUCB.bestChildUCB(mctsRoot, 0.1).parent_action);
    }
}