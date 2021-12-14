using System.Reflection;

namespace Qwirkle.Web.Api.Controllers;

[ApiController]
[Authorize] //todo : only for bot
[Route("Bot")]
public class BotController : ControllerBase
{
    private readonly BotUseCase _botUseCase;
    private readonly UserManager<UserDao> _userManager;
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private int UserId => int.Parse(_userManager.GetUserId(User) ?? "0");

    private MonteCarloTreeSearchNode mcts;

    public BotController(BotUseCase botUseCase, UserManager<UserDao> userManager)
    {
        _botUseCase = botUseCase;
        _userManager = userManager;
    }


    [HttpGet("PossibleMoves/{gameId:int}")]
    public ActionResult ComputeDoableMoves(int gameId)
    {
        _logger.Info($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");
        return new ObjectResult(_botUseCase.ComputeDoableMoves(gameId, UserId));
    }

    [HttpGet("BestMoves/{gameId:int}")]
    public ActionResult BestMoves(int gameId)
    {
        _logger.Info($"userId:{UserId} {MethodBase.GetCurrentMethod()!.Name} with {gameId}");

        mcts = new MonteCarloTreeSearchNode(_botUseCase.GetGame(gameId));
        var playReturns = _botUseCase.ComputeDoableMoves(gameId, UserId);
        var random = new Random();
        var playerIndexRoot = mcts.game.Players.FindIndex(player => player.IsTurn);
        var mctsroot = Expand.ExpandMcts(mcts, playReturns, playerIndexRoot);

        for (var i = 0; i < 5; i++)
        {
            mctsroot.children.ForEach(mcts =>
            {
                var searchPath = new List<MonteCarloTreeSearchNode>();
                var mcts_rollout = mcts;
                searchPath.Add(mctsroot);
                while (!mcts_rollout.game.GameOver)
                {
                    var player = mcts_rollout.game.Players.Where(player => player.IsTurn).FirstOrDefault();
                    var playerIndex = mcts_rollout.game.Players.FindIndex(player => player.IsTurn);
                    var playReturns = _botUseCase.ComputeDoableMovesMcts(mcts_rollout.game.Board, player, mcts_rollout.game);
                    if (playReturns.Count > 0)
                    {
                        mcts_rollout = Expand.ExpandMcts(mcts_rollout, playReturns, playerIndex);
                        var index = random.Next(playReturns.Count);
                        var coordinatesrandomAction = new List<PlayReturn> {playReturns[index]};
                        mcts_rollout.children[index].game.Players[playerIndex].Points += playReturns[index].Points;
                        searchPath.Add(mcts_rollout);
                        mcts_rollout.children[index].number_of_visits++;
                        mcts_rollout = new MonteCarloTreeSearchNode(mcts_rollout.children[index].game, mcts_rollout);
                    }
                    else
                    {
                        var removeTiles = mcts_rollout.game.Players[playerIndex].Rack.Tiles;
                        var indexRemoveTile = random.Next(removeTiles.Count);
                        for (var i = 0; i < indexRemoveTile; i++)
                        {
                            var removeTile = removeTiles[random.Next(mcts_rollout.game.Players[playerIndex].Rack.Tiles.Count)];

                            mcts_rollout.game.Players[playerIndex].Rack.Tiles.Remove(removeTile);
                            mcts_rollout.game.Bag.Tiles.Add(new TileOnBag(removeTile.Color, removeTile.Shape));
                        }
                        for (var i = 0; i < indexRemoveTile; i++)
                        {

                            var index = random.Next(mcts_rollout.game.Bag.Tiles.Count);
                            var addTile = mcts_rollout.game.Bag.Tiles[index];
                            mcts_rollout.game.Players[playerIndex].Rack.Tiles.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
                            mcts_rollout.game.Bag.Tiles.Remove(addTile);
                        }

                        mcts_rollout = Expand.SetNextPlayerTurnToPlay(mcts_rollout, mcts_rollout.game.Players[playerIndex]);
                        searchPath.Add(mcts_rollout);
                    }

                }

                var score = mcts_rollout.game.Players.Select(p => p.Points).ToList().Max();
                if (score == mcts_rollout.game.Players[playerIndexRoot].Points)
                {
                    mcts_rollout.wins++;
                }
                else
                {
                    mcts_rollout.looses++;
                }

                mcts_rollout = Backpropagate.backpropagate(mcts_rollout, searchPath[^1]);
                while (mcts_rollout.parent != null)
                {

                    mcts_rollout = mcts_rollout.parent;

                }

            });


        }
        var val = BestChildUCB.bestChildUCB(mctsroot, 0.1);
        return new ObjectResult(BestChildUCB.bestChildUCB(mctsroot, 0.1).parent_action);
    }
}