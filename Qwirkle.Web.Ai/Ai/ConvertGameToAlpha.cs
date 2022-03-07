


// using static Tensorflow.Binding;
// using static Tensorflow.KerasApi;
// using Tensorflow ;
// using NumSharp;
using TorchSharp;


namespace Qwirkle.Web.Ai;

public class ConvertGameToAlpha
{


    public torch.Tensor AffectColor(MonteCarloTreeSearchNode mcts, TileColor color, int index, torch.Tensor torColor)
    {





        mcts.Game.Board.Tiles.ToList().ForEach(t =>
        {
            if (t.ToTile().Color == color)
            {
                torColor[index][27 + t.Coordinates.X, 27 + t.Coordinates.Y] = torch.tensor(1f);
            }

        });

        return torColor;
    }
     public torch.Tensor AffectColorRack(Rack rack, TileColor color, int index, torch.Tensor torColor)
    {




var y=0;
        rack.Tiles.ToList().ForEach(t =>
        {
            if (t.ToTile().Color == color)
            {
                torColor[index][27 + 0, 27 + y] = torch.tensor(1f);
                y++;
            }

        });

        return torColor;
    }
    public torch.Tensor AffectEmpty(MonteCarloTreeSearchNode mcts, int index, torch.Tensor torEmpty)
    {

        long[,] array = new long[54, 54];
        for (int row = 0; row < 54; row++)
        {
            for (int column = 0; column < 54; column++)
            {
                torEmpty[index][row, column] = torch.tensor(1f);
            }
        }


        mcts.Game.Board.Tiles.ToList().ForEach(t =>
        {

            torEmpty[index][27 + t.Coordinates.X, 27 + t.Coordinates.Y] = torch.tensor(0f);
        });

        return torEmpty;
    }
    public torch.Tensor AffectShape(MonteCarloTreeSearchNode mcts, TileShape shape, int index, torch.Tensor torShape)
    {



        mcts.Game.Board.Tiles.ToList().ForEach(t =>
        {
            if (t.ToTile().Shape == shape)
            {
                torShape[index][27 + t.Coordinates.X, 27 + t.Coordinates.Y] = torch.tensor(1f);
            }
        });

        return torShape;
    }
     public torch.Tensor AffectShapeRack(Rack rack, TileShape shape, int index, torch.Tensor torShape)
    {

var y=0;

       rack.Tiles.ToList().ForEach(t =>
        {
            if (t.ToTile().Shape == shape)
            {
                torShape[index][27 + 0 ,27 + y] = torch.tensor(1f);
                y++;
            }
        });

        return torShape;
    }
    public torch.Tensor GetTensorBoardInput(MonteCarloTreeSearchNode mcts,Rack rack)
    {

                var torchInput = torch.zeros(26, 54, 54, dtype: torch.float32);

  for (int j = 0; j < 26; j++){
        for (int row = 0; row < 54; row++)
        {
            for (int column = 0; column < 54; column++)
            {
                torchInput[j][row, column] = torch.tensor(0f);
            }
        }}
        var index = 0;

        foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
        {
            torchInput = AffectColor(mcts, color, index, torchInput);

            index++;
        }
        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            torchInput = AffectShape(mcts, shape, index, torchInput);

            index++;
        }

        foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
        {
            torchInput = AffectColorRack(rack, color, index, torchInput);

            index++;
        }
        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            torchInput = AffectShapeRack(rack, shape, index, torchInput);

            index++;
        }
        torchInput = AffectEmpty(mcts, index, torchInput);





        return torchInput;



    }

}


