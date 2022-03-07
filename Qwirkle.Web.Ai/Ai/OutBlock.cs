using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;

namespace Qwirkle.Web.Ai;
public class OutBlock : nn.Module
{


    public OutBlock()
         : base(nameof(OutBlock))
    {
        RegisterComponents();
    }

    public List<Tensor> forwardTest(Tensor input)
    {

         var policy = input.view(-1,1200000);
        // var policy = flat.forward(input);

        policy = fc1.forward(policy);
        policy = nn.functional.relu(policy);
        policy = drp.forward(policy);
        policy = nn.functional.softmax(policy, 0);
        var value = input.view(-1, 640000);
        value = fc1.forward(value);
        value = nn.functional.relu(value);
        value = drp.forward(value);
        value = fc2.forward(value);
        value = nn.functional.relu(value);
        value = drp.forward(value);
        value = fclast.forward(value);
        value = torch.tanh(value);
        List<Tensor> T = new List<Tensor>();
        T.Add(policy);
        T.Add(value);
        return T;
    }

    private nn.Module fc1 = nn.Linear(1200000, 5000);
    private nn.Module fc2 = nn.Linear(5000, 500);


    private nn.Module fclast = nn.Linear(500, 1);

    private nn.Module flat = nn.Flatten();

    private nn.Module drp = nn.Dropout(0.3);




    // These don't have any parameters, so the only reason to instantiate
    // them is performance, since they will be used over and over.

    private nn.Module logsm = nn.LogSoftmax(1);

}

