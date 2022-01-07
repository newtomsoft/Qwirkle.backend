namespace Qwirkle.Domain.Services.Ai;

public class Expand
{


    public static MonteCarloTreeSearchNode ExpandMcts(MonteCarloTreeSearchNode mcts, List<PlayReturn> playReturns, int indexPlayer)
    {
        foreach (var coordinate in playReturns)
        {

            var newgame = new Game(mcts.Game);

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
            childrenNode = SetNextPlayerTurnToPlay(childrenNode, childrenNode.Game.Players[indexPlayer]);
            mcts.Children.Add(childrenNode);

        }



        return mcts;
    }


    public static MonteCarloTreeSearchNode SetNextPlayerTurnToPlay(MonteCarloTreeSearchNode mcts, Player player)
    {
        if (mcts.Game.GameOver) return mcts;


        if (mcts.Game.Players.Count == 1)
        {
            player.SetTurn(true);

        }
        else
        {
            var position = mcts.Game.Players.FirstOrDefault(p => p.Id == player.Id)!.GamePosition;
            var playersNumber = mcts.Game.Players.Count;
            var nextPlayerPosition = position < playersNumber ? position + 1 : 1;
            var nextPlayer = mcts.Game.Players.FirstOrDefault(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            nextPlayer!.SetTurn(true);

        }

        return mcts;
    }
}