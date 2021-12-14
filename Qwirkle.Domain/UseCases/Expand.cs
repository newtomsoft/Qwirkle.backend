namespace Qwirkle.Domain.UseCases;

public class Expand
{


    public static MonteCarloTreeSearchNode ExpandMcts(MonteCarloTreeSearchNode mcts, List<PlayReturn> playReturns, int indexPlayer)
    {
        foreach (var coordinate in playReturns)
        {

            var newgame = new Game(mcts.game);

            var random = new Random();
            coordinate.TilesPlayed.ForEach(tileBoard =>
            {

                newgame.Board.Tiles.Add(tileBoard);
                var removeTile = newgame.Players[indexPlayer].Rack.Tiles.Where(tile => tile.Color == tileBoard.Color && tile.Shape == tileBoard.Shape).FirstOrDefault();
                newgame.Players[indexPlayer].Rack.Tiles.Remove(removeTile);

                if (newgame.Bag.Tiles.Count > 0)
                {
                    var index = random.Next(newgame.Bag.Tiles.Count);
                    var addTile = newgame.Bag.Tiles[index];

                    newgame.Bag.Tiles.Remove(addTile);
                    newgame.Players[indexPlayer].Rack.Tiles.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
                }


            });
            var childrenNode = new MonteCarloTreeSearchNode(newgame, mcts, coordinate.TilesPlayed);
            childrenNode = SetNextPlayerTurnToPlay(childrenNode, childrenNode.game.Players[indexPlayer]);
            mcts.children.Add(childrenNode);

        }



        return mcts;
    }


    public static MonteCarloTreeSearchNode SetNextPlayerTurnToPlay(MonteCarloTreeSearchNode mcts, Player player)
    {
        if (mcts.game.GameOver) return mcts;


        if (mcts.game.Players.Count == 1)
        {
            player.SetTurn(true);

        }
        else
        {
            var position = mcts.game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = mcts.game.Players.Count;
            var nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            var nextPlayer = mcts.game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            nextPlayer!.SetTurn(true);

        }

        return mcts;
    }
}