using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;

namespace Qwirkle.Web.Ai;
public class ResBlock : nn.Module
{


    public ResBlock()
         : base(nameof(ResBlock))
    {
        RegisterComponents();
    }

    public override Tensor forward(Tensor input)
    {
        var residual = input;
        var l11 = bn1.forward(conv1.forward(input));

        var l12 = nn.functional.relu(l11);
        l12 = drp.forward(l12);

        var l21 = bn2.forward(conv2.forward(l12));
        var l22 = nn.functional.relu(l21);

        var l23 = drp.forward(l22);
        l23 += residual;
        l23 = nn.functional.relu(l23);


        return l23;
    }

    private nn.Module conv1 = nn.Conv2d(256, 2048, 1);
    private nn.Module bn1 = nn.BatchNorm2d(2048, 2048, 1);

    private nn.Module conv2 = nn.Conv2d(2048, 256, 1);
    private nn.Module bn2 = nn.BatchNorm2d(256, 256, 1);
    private nn.Module drp = nn.Dropout(0.3);



    // These don't have any parameters, so the only reason to instantiate
    // them is performance, since they will be used over and over.



}

