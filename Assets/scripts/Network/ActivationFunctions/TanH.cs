using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanH : IActivationFunction
{
    float IActivationFunction.Derivative(float z)
    {
        return (1 - z) * (1 + z);
    }

    float IActivationFunction.Func(float z)
    {
        return System.MathF.Tanh(z);
    }

    string IActivationFunction.ToString()
    {
        return "TanH";
    }
}
