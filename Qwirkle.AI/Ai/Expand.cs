using Qwirkle.Domain.Services;
using Qwirkle.Domain.Services.Ai;

namespace Qwirkle.Domain.UseCases.Ai;

public class Expand
{


    public MonteCarloTreeSearchNode ExpandMcts(MonteCarloTreeSearchNode mcts, List<PlayReturn> playReturns, int indexPlayer)
    {
        foreach (var coordinate in playReturns)
        {

            var newgame = new Game(mcts.Game);

            var random = new Random();
            coordinate.TilesPlayed.ToList().ForEach(tileBoard =>
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
            var childrenNode = new MonteCarloTreeSearchNode(newgame, mcts, coordinate.TilesPlayed.ToHashSet());
            childrenNode = SetNextPlayerTurnToPlay(childrenNode, childrenNode.Game.Players[indexPlayer]);
            mcts.Children.Add(childrenNode);

        }



        return mcts;
    }
    public MonteCarloTreeSearchNode ExpandMctsOne(MonteCarloTreeSearchNode mcts, PlayReturn coordinate, int indexPlayer)
    {


        var newgame = new Game(mcts.Game);

        var random = new Random();


        newgame.Board.Tiles.Add(coordinate.TilesPlayed.First());
        var removeTile = newgame.Players[indexPlayer].Rack.Tiles.Where(tile => tile.Color == coordinate.TilesPlayed.First().Color && tile.Shape == coordinate.TilesPlayed.First().Shape).FirstOrDefault();
        newgame.Players[indexPlayer].Rack.Tiles.Remove(removeTile);

        if (newgame.Bag.Tiles.Count > 0)
        {
            var index = random.Next(newgame.Bag.Tiles.Count);
            var addTile = newgame.Bag.Tiles[index];

            newgame.Bag.Tiles.Remove(addTile);
            newgame.Players[indexPlayer].Rack.Tiles.Add(new TileOnPlayer(0, addTile.Color, addTile.Shape));
        }




        var childrenNode = new MonteCarloTreeSearchNode(newgame, mcts, coordinate.TilesPlayed.ToHashSet());


        childrenNode = SetNextPlayerTurnToPlay(childrenNode, childrenNode.Game.Players[indexPlayer]);
        mcts.Children.Add(childrenNode);





        return mcts;
    }


    public MonteCarloTreeSearchNode SetNextPlayerTurnToPlay(MonteCarloTreeSearchNode mcts, Player player)
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
            var nextPlayerPosition = position < playersNumber - 1 ? position + 1 : 0;
            var nextPlayer = mcts.Game.Players.First(p => p.GamePosition == nextPlayerPosition);
            player.SetTurn(false);
            nextPlayer.SetTurn(true);


        }

        return mcts;
    }
    public static PlayReturn TryPlayTilesSimulationMCTS(Player player, HashSet<TileOnBoard> tilesToPlay, Game game) => GetPlayReturnMCTS(tilesToPlay, player, game);
    public List<PlayReturn> ComputeDoableMovesMcts(Board board, Player player, Game game, int selectFirst)
    {

        var rack = player.Rack.WithoutDuplicatesTiles();
        var random = new Random();
        var boardAdjoiningCoordinates = game.Board.GetFreeAdjoiningCoordinatesToTiles();
        if (selectFirst > 10)
        {
            var index = random.Next(boardAdjoiningCoordinates.Count);
            boardAdjoiningCoordinates = boardAdjoiningCoordinates.GetRange(index, 1);
        }

        var allPlayReturns = new List<PlayReturn>();
        var playReturnsWith1Tile = new HashSet<PlayReturn>();
        foreach (var coordinates in boardAdjoiningCoordinates)
        {
            for (int i = 0; i < rack.Tiles.Count; i++)
            {
                TileOnPlayer tile = rack.Tiles[i];
                var playReturn = TryPlayTilesSimulationMCTS(player, new HashSet<TileOnBoard> { TileOnBoard.From(tile, coordinates) }, game);
                if (playReturn.Code == ReturnCode.Ok) playReturnsWith1Tile.Add(playReturn);
            }
        };

        allPlayReturns.AddRange(playReturnsWith1Tile);
        var lastPlayReturn = playReturnsWith1Tile;
        for (var tilePlayedNumber = 2; tilePlayedNumber <= 6; tilePlayedNumber++)
        {
            var currentPlayReturns = new HashSet<PlayReturn>();
            foreach (var playReturn in lastPlayReturn)
            {
                var tilesPlayed = playReturn.TilesPlayed;
                var currentTilesToTest = rack.Tiles.Select(t => t.ToTile()).Except(tilesPlayed.Select(tP => tP.ToTile())).Select((t, index) => t.ToTileOnPlayer((RackPosition)index)).ToList();
                var firstGameMove = game.Board.Tiles.Count == 0;
                if (firstGameMove && tilePlayedNumber == 2) // todo ok but can do better
                {
                    currentPlayReturns.UnionWith(ComputePlayReturnInRow(RandomRowType(), player, boardAdjoiningCoordinates, currentTilesToTest, tilesPlayed.ToHashSet(), true, game));
                }
                else
                {
                    foreach (RowType rowType in Enum.GetValues(typeof(RowType)))
                        currentPlayReturns.UnionWith(ComputePlayReturnInRow(rowType, player, boardAdjoiningCoordinates, currentTilesToTest, tilesPlayed.ToHashSet(), false, game));
                }
            }
            allPlayReturns.AddRange(currentPlayReturns);
            lastPlayReturn = currentPlayReturns;
        }
        return allPlayReturns.OrderByDescending(x => x.Points).ToList();
    }

    private static RowType RandomRowType()
    {
        var rowTypeValues = typeof(RowType).GetEnumValues();
        var index = new Random().Next(rowTypeValues.Length);
        return (RowType)rowTypeValues.GetValue(index)!;
    }


    public static List<T> GetRandomFromList<T>(List<T> passedList, int numberToChoose)
    {
        if (numberToChoose == 0) return passedList;
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



    private static IEnumerable<PlayReturn> ComputePlayReturnInRow(RowType rowType, Player player, IEnumerable<Coordinates> boardAdjoiningCoordinates, List<TileOnPlayer> tilesToTest, HashSet<TileOnBoard> tilesAlreadyPlayed, bool firstGameMove, Game game)
    {
        int tilesPlayedNumber = tilesAlreadyPlayed.Count;
        var coordinatesPlayed = tilesAlreadyPlayed.Select(tilePlayed => tilePlayed.Coordinates).ToList();

        sbyte coordinateChangingMin, coordinateChangingMax;
        var firstTilePlayedX = coordinatesPlayed[0].X;
        var firstTilePlayedY = coordinatesPlayed[0].Y;
        if (tilesPlayedNumber >= 2)
        {
            coordinateChangingMax = rowType is RowType.Line ? coordinatesPlayed.Max(c => c.X) : coordinatesPlayed.Max(c => c.Y);
            coordinateChangingMin = rowType is RowType.Line ? coordinatesPlayed.Min(c => c.X) : coordinatesPlayed.Min(c => c.Y);
        }
        else
        {
            coordinateChangingMax = rowType is RowType.Line ? firstTilePlayedX : firstTilePlayedY;
            coordinateChangingMin = coordinateChangingMax;
        }

        var coordinateFixed = rowType is RowType.Line ? coordinatesPlayed.First().Y : coordinatesPlayed.First().X;

        var playReturns = new List<PlayReturn>();
        var boardAdjoiningCoordinatesRow = rowType is RowType.Line ?
                                            boardAdjoiningCoordinates.Where(c => c.Y == coordinateFixed).Select(c => (int)c.X).ToList()
                                          : boardAdjoiningCoordinates.Where(c => c.X == coordinateFixed).Select(c => (int)c.Y).ToList();

        if (!firstGameMove)
        {
            if (coordinateChangingMax >= boardAdjoiningCoordinatesRow.Max()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + 1);
            if (coordinateChangingMin <= boardAdjoiningCoordinatesRow.Min()) boardAdjoiningCoordinatesRow.Add(coordinateChangingMin - 1);
        }
        else
        {
            var addOrSubtract1Unit = new Random().Next(2) * 2 - 1;
            boardAdjoiningCoordinatesRow.Add(coordinateChangingMax + addOrSubtract1Unit);
            // we have coordinateChangingMax = coordinateChangingMin
        }
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMax);
        boardAdjoiningCoordinatesRow.Remove(coordinateChangingMin);

        foreach (var currentCoordinate in boardAdjoiningCoordinatesRow)
        {
            foreach (var tile in tilesToTest)
            {
                var testedCoordinates = rowType is RowType.Line ? Coordinates.From(currentCoordinate, coordinateFixed) : Coordinates.From(coordinateFixed, currentCoordinate);
                var testedTile = TileOnBoard.From(tile, testedCoordinates);
                var currentTilesToTest = new HashSet<TileOnBoard>();
                currentTilesToTest.UnionWith(tilesAlreadyPlayed);
                currentTilesToTest.Add(testedTile);
                var playReturn = TryPlayTilesSimulationMCTS(player, currentTilesToTest, game);
                if (playReturn.Code == ReturnCode.Ok) playReturns.Add(playReturn);
            }
        }
        return playReturns;
    }
    public static PlayReturn GetPlayReturnMCTS(HashSet<TileOnBoard> tilesPlayed, Player player, Game game)
    {
        if (game.Board.Tiles.Count == 0 && tilesPlayed.Count == 1) return new PlayReturn(game.Id, ReturnCode.Ok, tilesPlayed.ToList(), null, 1);
        if (IsCoordinatesNotFree()) return new PlayReturn(game.Id, ReturnCode.NotFree, null, null, 0);
        if (IsBoardNotEmpty() && IsAnyTileIsolated()) return new PlayReturn(game.Id, ReturnCode.TileIsolated, null, null, 0);
        // var wonPoints = computePointsUseCase.ComputePointsMcts(tilesPlayed, game);
        var wonPoints = ComputePoints.Compute(game, tilesPlayed);
        if (wonPoints == 0) return new PlayReturn(game.Id, ReturnCode.TilesDoesntMakedValidRow, null, null, 0);

        if (IsGameFinished())
        {
            const int endGameBonusPoints = 6;
            wonPoints += endGameBonusPoints;

            game = new Game(game.Id, game.Board, game.Players, true);
        }
        return new PlayReturn(game.Id, ReturnCode.Ok, tilesPlayed.ToList(), null, wonPoints);

        bool IsGameFinished() => IsBagEmpty() && AreAllTilesInRackPlayed();
        bool AreAllTilesInRackPlayed() => tilesPlayed.Count == player.Rack.Tiles.Count;
        bool IsBagEmpty() => game.Bag?.Tiles.Count == 0;
        bool IsBoardNotEmpty() => game.Board.Tiles.Count > 0;
        bool IsAnyTileIsolated() => !tilesPlayed.Any(tile => game.Board.IsIsolatedTile(tile));
        bool IsCoordinatesNotFree() => tilesPlayed.Any(tile => !game.Board.IsFreeTile(tile));
    }
    public MonteCarloTreeSearchNode SwapTilesMcts(MonteCarloTreeSearchNode mcts, int playerIndex)
    {
        var random = new Random();
        var rackToSwap = mcts.Game.Players[playerIndex].Rack.Tiles;
        if (rackToSwap.Count > 0)
        {
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
        }


        return mcts;
    }







}