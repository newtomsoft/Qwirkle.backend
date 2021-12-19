namespace Qwirkle.Domain.UseCases.Ai;

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
public static MonteCarloTreeSearchNode ExpandMctsOne(MonteCarloTreeSearchNode mcts, PlayReturn coordinate, int indexPlayer)
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
    public static PlayReturn TryPlayTilesSimulationMCTS(Player player, List<TileOnBoard> tilesToPlay, Game game) => GetPlayReturnMCTS(tilesToPlay, player, game);
   
    public static List<PlayReturn> ComputeDoableMovesMcts(Board board, Player player, Game game)
    {

        var rack = player.Rack.WithoutDuplicatesTiles();


        var boardAdjoiningCoordinates =board.GetFreeAdjoiningCoordinatesToTiles().Take(15);
        
        var playReturnsWith1Tile = new List<PlayReturn>();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            foreach (var tile in rack.Tiles)
            {
                var playReturn = TryPlayTilesSimulationMCTS(player, new List<TileOnBoard> { TileOnBoard.From(tile, coordinates) }, game);
                if (playReturn.Code == PlayReturnCode.Ok) playReturnsWith1Tile.Add(playReturn);
            }
        }
        playReturnsWith1Tile = playReturnsWith1Tile.OrderByDescending(p => p.Points).ToList();
       
        var playReturnsWith2Tiles = new List<PlayReturn>();
        foreach (var playReturn in playReturnsWith1Tile)
        {
            var tilePlayed = playReturn.TilesPlayed[0];
            var currentTilesToTest = rack.Tiles.Where(t => t != tilePlayed).ToList();

            var firstGameMove = board.Tiles.Count == 0;
            if (firstGameMove)
            {
                playReturnsWith2Tiles.AddRange(ComputePlayReturnWith2TilesInRow(RandomRowType(), player, boardAdjoiningCoordinates, currentTilesToTest, tilePlayed, true, game));
            }
            else
            {
                foreach (RowType rowType in Enum.GetValues(typeof(RowType)))
                    playReturnsWith2Tiles.AddRange(ComputePlayReturnWith2TilesInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, tilePlayed, false, game));
            }
        }
        playReturnsWith2Tiles = playReturnsWith2Tiles.OrderBy(p => p.Points).ToList();


        var playReturnsWith3Tiles = new List<PlayReturn>();
        foreach (var playReturn in playReturnsWith2Tiles)
        {
            var firstTilePlayed = playReturn.TilesPlayed[0];
            var secondTilePlayed = playReturn.TilesPlayed[1];
            var rowType = firstTilePlayed.Coordinates.X == secondTilePlayed.Coordinates.X ? RowType.Column : RowType.Line;

            var currentTilesToTest = rack.Tiles.Where(t => t != firstTilePlayed && t != secondTilePlayed).ToList();
            playReturnsWith3Tiles.AddRange(ComputePlayReturnWith3TilesInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, firstTilePlayed, secondTilePlayed, game));
        }

        //we have all possible moves with 3 tiles :)
        var allPlayReturns = new List<PlayReturn>();
        allPlayReturns.AddRange(playReturnsWith3Tiles);
        allPlayReturns.AddRange(playReturnsWith2Tiles);
        allPlayReturns.AddRange(playReturnsWith1Tile);
        
        return allPlayReturns;


        static RowType RandomRowType()
        {
            var rowTypeValues = typeof(RowType).GetEnumValues();
            var index = new Random().Next(rowTypeValues.Length);
            return (RowType)rowTypeValues.GetValue(index)!;
        }
    }
       public static List<T> GetRandomFromList<T>(List<T> passedList, int numberToChoose)
{
    if (numberToChoose==0) return passedList;
    System.Random rnd = new System.Random();
    List<T> chosenItems = new List<T>();

    for (int i = 1; i <= numberToChoose; i++)
    {
      int index = rnd.Next(passedList.Count);
      chosenItems.Add(passedList[index]);
    }

    //Debug.Log(chosenItems.Count);

    return chosenItems;
}
    private static IEnumerable<PlayReturn> ComputePlayReturnWith2TilesInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> tilesToTest, TileOnBoard firstTile, bool firstGameMove,Game game)
    {
        var (tilePlayedX, tilePlayedY) = firstTile.Coordinates;
        var coordinateChanging = rowType is RowType.Line ? tilePlayedX : tilePlayedY;
        var coordinateFixed = rowType is RowType.Line ? tilePlayedY : tilePlayedX;
        var playReturnsWith2Tiles = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
                                            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
                                          : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (!firstGameMove)
        {
            if (coordinateChanging == boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChanging + 1);
            if (coordinateChanging == boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChanging - 1);
        }
        else
        {
            var addOrSubtract1Unit = new Random().Next(2) * 2 - 1;
            boardAdjoiningCoordinatesRow.Add(coordinateChanging + addOrSubtract1Unit);
        }

        boardAdjoiningCoordinatesRow.Remove(coordinateChanging);
        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in tilesToTest)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var playReturn2 = TryPlayTilesSimulationMCTS(player, new List<TileOnBoard> { firstTile, testedTile },game);
                
                if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith2Tiles.Add(playReturn2);
            }
        }
        return playReturnsWith2Tiles;
    }

    private static IEnumerable<PlayReturn> ComputePlayReturnWith3TilesInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> rackTiles, TileOnBoard firstTile, TileOnBoard secondTile, Game game)
    {
        var (firstTilePlayedX, firstTilePlayedY) = firstTile.Coordinates;
        var (secondTilePlayedX, secondTilePlayedY) = secondTile.Coordinates;

        var coordinateChangingMax = rowType is RowType.Line ? Math.Max(firstTilePlayedX, secondTilePlayedX) : Math.Max(firstTilePlayedY, secondTilePlayedY);
        var coordinateChangingMin = rowType is RowType.Line ? Math.Min(firstTilePlayedX, secondTilePlayedX) : Math.Min(firstTilePlayedY, secondTilePlayedY);

        var coordinateFixed = rowType is RowType.Line ? firstTilePlayedY : firstTilePlayedX;
        var playReturnsWith3Tiles = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
            : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (coordinateChangingMax == boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + 1);
        if (coordinateChangingMin == boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMin - 1);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMax);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMin);

        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in rackTiles)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var playReturn2 = TryPlayTilesSimulationMCTS(player, new List<TileOnBoard> { firstTile, secondTile, testedTile },game);
                
                if (playReturn2.Code == PlayReturnCode.Ok) playReturnsWith3Tiles.Add(playReturn2);
            }
        }
        return playReturnsWith3Tiles;
    }
  
     public static PlayReturn GetPlayReturnMCTS(List<TileOnBoard> tilesPlayed, Player player, Game game)
    {
        if (game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, 1);
        if (IsCoordinatesNotFree()) return new PlayReturn(game.Id, PlayReturnCode.NotFree, null, null, 0);
        if (IsBoardNotEmpty() && IsAnyTileIsolated()) return new PlayReturn(game.Id, PlayReturnCode.TileIsolated, null, null, 0);
        var computePointsUseCase = new ComputePointsUseCase();
        var wonPoints = computePointsUseCase.ComputePointsMcts(tilesPlayed, game);

        if (wonPoints == 0) return new PlayReturn(game.Id, PlayReturnCode.TilesDoesntMakedValidRow, null, null, 0);

        if (IsGameFinished())
        {
            const int endGameBonusPoints = 6;
            wonPoints += endGameBonusPoints;

            game.GameOver = true;
        }
        return new PlayReturn(game.Id, PlayReturnCode.Ok, tilesPlayed, null, wonPoints);

        bool IsGameFinished() => IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsBagEmpty() => game.Bag?.Tiles.Count == 0;
        bool IsBoardNotEmpty() => game.Board.Tiles.Count > 0;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !game.Board.IsFreeTile(tile));
    }
    public static MonteCarloTreeSearchNode SwapTilesMcts(MonteCarloTreeSearchNode mcts, int playerIndex)
    {
        var random = new Random();
        var rackToSwap = mcts.Game.Players[playerIndex].Rack.Tiles;
        var tileNumberToSwap = random.Next(rackToSwap.Count) + 1;
        for (var i = 0; i < tileNumberToSwap; i++)
        {
            var removeTile = rackToSwap[random.Next(rackToSwap.Count)];

            rackToSwap.Remove(removeTile);
            mcts.Game.Bag.Tiles.Add(new TileOnBag(removeTile.Color, removeTile.Shape));
        }
        for (var i = 0; i < tileNumberToSwap; i++)
        {
            var index = random.Next(mcts.Game.Bag.Tiles.Count);
            var addTile = mcts.Game.Bag.Tiles[index];
            rackToSwap.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
            mcts.Game.Bag.Tiles.Remove(addTile);
        }


        
        return mcts;
    }


   

   

   
}