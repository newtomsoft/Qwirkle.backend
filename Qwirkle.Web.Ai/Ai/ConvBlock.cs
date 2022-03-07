using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;

namespace Qwirkle.Web.Ai;
public class ConvBlock : nn.Module
{


    public ConvBlock()
         : base(nameof(ConvBlock))
    {
        RegisterComponents();
    }

    public override Tensor forward(Tensor input)

    {
        var convTest = conv1.forward(torch.unsqueeze(input, 0));
        var l11 = bn1.forward(convTest);
        var l12 = nn.functional.relu(l11);

        var l21 = bn2.forward(conv2.forward(l12));
        var l22 = nn.functional.relu(l21);


        return l22;
    }

    private nn.Module conv1 = nn.Conv2d(26, 1024,3);
    private nn.Module bn1 = nn.BatchNorm2d(1024, 1024, 3);

    private nn.Module conv2 = nn.Conv2d(1024, 256, 3);
    private nn.Module bn2 = nn.BatchNorm2d(256, 256, 3);


    // These don't have any parameters, so the only reason to instantiate
    // them is performance, since they will be used over and over.



}

