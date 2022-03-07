
using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;
using Newtonsoft.Json;

namespace Qwirkle.Web.Ai.Ai;

public class TestAlphaLoss
{
    private readonly AlphaLoss connectionFactory;
    private List<List<TileOnBoard>> listTile;
    public TestAlphaLoss()
    {

        connectionFactory = new AlphaLoss();



    }


    [Fact]
    public void ShouldAlphaLossReturnZero()
    {

        var x = torch.tensor(new float[] { 1f, 1f, 1f, 1f, 1f });
        var y = torch.tensor(new float[] { 1f, 1f, 1f, 1f, 1f });
        var xvalue = torch.tensor(new float[] { 0f, 1f, 1f, 1f, 1f });
        var ypolicy = torch.tensor(new float[] { 1f, 0f, 1f, 1f, 1f });
        var result = connectionFactory.forwardAlphaLoss(x, xvalue, y, ypolicy);
        Assert.Equal(result, 0.49999997f);
    }


   
    [Fact]
    public void ShouldCreateTensorAllCombinaisonOfQwirkle()
    {
        List<List<TileOnBoard>> tiles = Qwirkle();
        tiles.AddRange(FiveQwirkle());
        tiles.AddRange(ThreeQwirkle());
        tiles.AddRange(TwoQwirkle());
        tiles.AddRange(OneQwirkle());


        Assert.Equal(6, tiles[0].Count);
        Assert.Equal(1, tiles[42875].Count);
        Assert.Equal(42876, tiles.Count);


    }

    private static List<List<TileOnBoard>> Qwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color1 in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                var data2 = Enum
                    .GetValues(typeof(TileColor))
                    .Cast<TileColor>()
                     .Where(item => item != color1).ToArray();
                foreach (TileColor color2 in data2)
                {
                    var data3 = data2
                        .Cast<TileColor>()
                         .Where(item => item != color2).ToArray();

                    foreach (TileColor color3 in data3)
                    {
                        var data4 = data3
                            .Cast<TileColor>()
                             .Where(item => item != color3).ToArray();
                        foreach (TileColor color4 in data4)
                        {
                            var data5 = data4
                                .Cast<TileColor>()
                                 .Where(item => item != color4).ToArray();
                            foreach (TileColor color5 in data5)
                            {
                                var data6 = data5
                                    .Cast<TileColor>()
                                     .Where(item => item != color5).ToArray();


                                foreach (TileColor color in data6)
                                {
                                    var TilesQwirkle = new List<TileOnBoard>();
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color1, shape), Coordinates.From(0, 0)));
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color2, shape), Coordinates.From(0, 0)));
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color3, shape), Coordinates.From(0, 0)));
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color4, shape), Coordinates.From(0, 0)));
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color5, shape), Coordinates.From(0, 0)));
                                    TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));

                                    tiles.Add(TilesQwirkle);

                                }
                            }
                        }

                    }
                }


            }
            foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))

            {
                foreach (TileShape shape1 in (TileShape[])Enum.GetValues(typeof(TileShape)))

                {
                    var data2 = Enum
                        .GetValues(typeof(TileShape))
                        .Cast<TileShape>()
                         .Where(item => item != shape1).ToArray();
                    foreach (TileShape shape2 in data2)
                    {
                        var data3 = data2
                            .Cast<TileShape>()
                             .Where(item => item != shape2).ToArray();

                        foreach (TileShape shape3 in data3)
                        {
                            var data4 = data3
                                .Cast<TileShape>()
                                 .Where(item => item != shape3).ToArray();
                            foreach (TileShape shape4 in data4)
                            {
                                var data5 = data4
                                    .Cast<TileShape>()
                                     .Where(item => item != shape4).ToArray();
                                foreach (TileShape shape5 in data5)
                                {
                                    var data6 = data5
                                        .Cast<TileShape>()
                                         .Where(item => item != shape5).ToArray();
                                    foreach (TileShape shapeq in data6)
                                    {
                                        var TilesQwirkle = new List<TileOnBoard>();
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape1), Coordinates.From(0, 0)));
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape2), Coordinates.From(0, 0)));
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape3), Coordinates.From(0, 0)));
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape4), Coordinates.From(0, 0)));
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape5), Coordinates.From(0, 0)));
                                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shapeq), Coordinates.From(0, 0)));
                                        tiles.Add(TilesQwirkle);

                                    }

                                }
                            }
                        }
                    }
                }
            }


        }

        return tiles;
    }

    [Fact]
    public void ShouldCreateTensorAllCombinaisonOfFiveQwirkle()
    {
        List<List<TileOnBoard>> tiles = FiveQwirkle();

        Assert.Equal(5, tiles[0].Count);
        Assert.Equal(5, tiles[1].Count);
        Assert.Equal(10800, tiles.Count);

    }

    private static List<List<TileOnBoard>> FiveQwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color1 in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                var data2 = Enum
                    .GetValues(typeof(TileColor))
                    .Cast<TileColor>()
                     .Where(item => item != color1).ToArray();
                foreach (TileColor color2 in data2)
                {
                    var data3 = data2
                        .Cast<TileColor>()
                         .Where(item => item != color2).ToArray();

                    foreach (TileColor color3 in data3)
                    {
                        var data4 = data3
                            .Cast<TileColor>()
                             .Where(item => item != color3).ToArray();
                        foreach (TileColor color4 in data4)
                        {
                            var data5 = data4
                                .Cast<TileColor>()
                                 .Where(item => item != color4).ToArray();

                            foreach (TileColor color in data4)
                            {
                                var TilesQwirkle = new List<TileOnBoard>();
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color1, shape), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color2, shape), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color3, shape), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color4, shape), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));

                                tiles.Add(TilesQwirkle);

                            }
                        }
                    }

                }
            }


        }
        foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))

        {
            foreach (TileShape shape1 in (TileShape[])Enum.GetValues(typeof(TileShape)))

            {
                var data2 = Enum
                    .GetValues(typeof(TileShape))
                    .Cast<TileShape>()
                     .Where(item => item != shape1).ToArray();
                foreach (TileShape shape2 in data2)
                {
                    var data3 = data2
                        .Cast<TileShape>()
                         .Where(item => item != shape2).ToArray();

                    foreach (TileShape shape3 in data3)
                    {
                        var data4 = data3
                            .Cast<TileShape>()
                             .Where(item => item != shape3).ToArray();
                        foreach (TileShape shape4 in data4)
                        {
                            var data5 = data4
                                .Cast<TileShape>()
                                 .Where(item => item != shape4).ToArray();
                            foreach (TileShape shape in data5)
                            {
                                var TilesQwirkle = new List<TileOnBoard>();
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape1), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape2), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape3), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape4), Coordinates.From(0, 0)));
                                TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
                                tiles.Add(TilesQwirkle);

                            }

                        }
                    }
                }
            }


        }

        return tiles;
    }

    [Fact]

    public void ShouldCreateTensorAllCombinaisonOfFourQwirkle()
    {
        List<List<TileOnBoard>> tiles = FourQwirkle();

        Assert.Equal(4, tiles[0].Count);
        Assert.Equal(4, tiles[4319].Count);
        Assert.Equal(4320, tiles.Count);

    }

    private static List<List<TileOnBoard>> FourQwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color1 in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                var data2 = Enum
                    .GetValues(typeof(TileColor))
                    .Cast<TileColor>()
                     .Where(item => item != color1).ToArray();
                foreach (TileColor color2 in data2)
                {
                    var data3 = data2
                        .Cast<TileColor>()
                         .Where(item => item != color2).ToArray();

                    foreach (TileColor color3 in data3)
                    {
                        var data4 = data3
                            .Cast<TileColor>()
                             .Where(item => item != color3).ToArray();

                        foreach (TileColor color in data4)
                        {
                            var TilesQwirkle = new List<TileOnBoard>();
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color1, shape), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color2, shape), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color3, shape), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));

                            tiles.Add(TilesQwirkle);

                        }
                    }

                }
            }


        }
        foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))

        {
            foreach (TileShape shape1 in (TileShape[])Enum.GetValues(typeof(TileShape)))

            {
                var data2 = Enum
                    .GetValues(typeof(TileShape))
                    .Cast<TileShape>()
                     .Where(item => item != shape1).ToArray();
                foreach (TileShape shape2 in data2)
                {
                    var data3 = data2
                        .Cast<TileShape>()
                         .Where(item => item != shape2).ToArray();

                    foreach (TileShape shape3 in data3)
                    {
                        var data4 = data3
                            .Cast<TileShape>()
                             .Where(item => item != shape3).ToArray();
                        foreach (TileShape shape in data4)
                        {
                            var TilesQwirkle = new List<TileOnBoard>();
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape1), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape2), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape3), Coordinates.From(0, 0)));
                            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
                            tiles.Add(TilesQwirkle);

                        }

                    }
                }
            }


        }

        return tiles;
    }

    [Fact]

    public void ShouldCreateTensorAllCombinaisonOfthreeQwirkle()
    {
        List<List<TileOnBoard>> tiles = ThreeQwirkle();

        Assert.Equal(3, tiles[0].Count);
        Assert.Equal(3, tiles[1439].Count);
        Assert.Equal(1440, tiles.Count);

    }

    private static List<List<TileOnBoard>> ThreeQwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color1 in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                var data2 = Enum
                    .GetValues(typeof(TileColor))
                    .Cast<TileColor>()
                     .Where(item => item != color1).ToArray();
                foreach (TileColor color2 in data2)
                {
                    var data3 = data2
                        .Cast<TileColor>()
                         .Where(item => item != color2).ToArray();


                    foreach (TileColor color in data3)
                    {
                        var TilesQwirkle = new List<TileOnBoard>();
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color1, shape), Coordinates.From(0, 0)));
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color2, shape), Coordinates.From(0, 0)));
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
                        tiles.Add(TilesQwirkle);

                    }

                }
            }


        }
        foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))

        {
            foreach (TileShape shape1 in (TileShape[])Enum.GetValues(typeof(TileShape)))

            {
                foreach (TileShape shape2 in Enum
                    .GetValues(typeof(TileShape))
                    .Cast<TileShape>()
                     .Where(item => item != shape1).ToArray())
                {


                    foreach (TileShape shape in Enum
                    .GetValues(typeof(TileShape))
                    .Cast<TileShape>()
                     .Where(item => item != shape1).ToArray().Cast<TileShape>()
                         .Where(item => item != shape2).ToArray())
                    {
                        var TilesQwirkle = new List<TileOnBoard>();
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape1), Coordinates.From(0, 0)));
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape2), Coordinates.From(0, 0)));
                        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
                        tiles.Add(TilesQwirkle);

                    }

                }
            }


        }

        return tiles;
    }

    [Fact]

    public void ShouldCreateTensorAllCombinaisonOfTwoQwirkle()
    {
        List<List<TileOnBoard>> tiles = TwoQwirkle();

        Assert.Equal(2, tiles[0].Count);
        Assert.Equal(2, tiles[359].Count);
        Assert.Equal(360, tiles.Count);

    }

    private static List<List<TileOnBoard>> TwoQwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                TwoQwirkleAddTilesColor(tiles, shape, color);
                TwoQwirkleAddTilesShape(tiles, shape,color);
            }

        }

        return tiles;
    }

    private static void TwoQwirkleAddTilesColor(List<List<TileOnBoard>> tiles, TileShape shape, TileColor color1)
    {


        foreach (TileColor color in Enum
            .GetValues(typeof(TileColor))
            .Cast<TileColor>()
             .Where(item => item != color1).ToArray())
        {
            var TilesQwirkle = new List<TileOnBoard>();
            TilesQwirkle.Add(new TileOnBoard(new Tile(color1, shape), Coordinates.From(0, 0)));
            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
            tiles.Add(TilesQwirkle);
        }
    }

    private static void TwoQwirkleAddTilesShape(List<List<TileOnBoard>> tiles,  TileShape shape1,TileColor color)
    {
        foreach (TileShape shape in Enum
            .GetValues(typeof(TileShape))
            .Cast<TileShape>()
             .Where(item => item != shape1).ToArray())
        {
            var TilesQwirkle = new List<TileOnBoard>();
            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape1), Coordinates.From(0, 0)));
            TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
            tiles.Add(TilesQwirkle);
        }
    }

    [Fact]

    public void ShouldCreateTensorAllCombinaisonOfOneQwirkle()
    {
        List<List<TileOnBoard>> tiles = OneQwirkle();

        Assert.Equal(1, tiles[0].Count);
        Assert.Equal(1, tiles[35].Count);
        Assert.Equal(36, tiles.Count);

    }

    private static List<List<TileOnBoard>> OneQwirkle()
    {
        var tiles = new List<List<TileOnBoard>>();

        foreach (TileShape shape in (TileShape[])Enum.GetValues(typeof(TileShape)))
        {
            foreach (TileColor color in (TileColor[])Enum.GetValues(typeof(TileColor)))
            {
                OneQwirkleAddtiles(tiles, shape, color);
            }

        }

        return tiles;
    }

    private static void OneQwirkleAddtiles(List<List<TileOnBoard>> tiles, TileShape shape, TileColor color)
    {
        var TilesQwirkle = new List<TileOnBoard>();
        TilesQwirkle.Add(new TileOnBoard(new Tile(color, shape), Coordinates.From(0, 0)));
        tiles.Add(TilesQwirkle);
    }
}
