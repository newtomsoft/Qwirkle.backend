using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;

namespace Qwirkle.Web.Ai;
public class ConnectNet : nn.Module
{
    List<ResBlock> forwardTabResBlock;

    public ConnectNet()
         : base(nameof(ConnectNet))
    {
        RegisterComponents();
        forwardTabResBlock = createResblockProperty();
    }

    public List<Tensor> forwardTest(Tensor input)
    {


        var l11 = conv.forward(input);

        for (int i = 0; i < 20; i++)
        {
            l11 = forwardTabResBlock[i].forward(l11);
        }



        return outblock.forwardTest(l11);
    }

    private ConvBlock conv = new ConvBlock();

    private List<ResBlock> createResblockProperty()
    {
        var tabResBlock = new List<ResBlock>();
        for (int i = 0; i < 20; i++)
        {
            tabResBlock.Add(new ResBlock());
        }

        return tabResBlock;
    }

    private OutBlock outblock = new OutBlock();


    // These don't have any parameters, so the only reason to instantiate
    // them is performance, since they will be used over and over.



}

