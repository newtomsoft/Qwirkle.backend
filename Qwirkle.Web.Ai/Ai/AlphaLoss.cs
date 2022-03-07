using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.TensorExtensionMethods;
using static TorchSharp.torch.distributions;


namespace Qwirkle.Web.Ai;
public class AlphaLoss : nn.Module
{
    public AlphaLoss()
            : base(nameof(AlphaLoss))
    {
        RegisterComponents();
    }
    public float forwardAlphaLoss(Tensor y_value, Tensor value, Tensor y_policy, Tensor policy)

    {
        var value_error = 0f;
        var policy_error = 0f;
        for (var index = 0; index < y_value.numel(); index++)
        {
            value_error += (float)Math.Pow(((float)value[index] - (float)y_value[index]), 2);
            policy_error += (float)Math.Log((-(float)policy[index] * 1e-8 + (float)y_policy[index]));

        }

        var total_error = (value_error + policy_error) / 2;
        return total_error;
    }


}
