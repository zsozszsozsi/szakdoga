using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanH : IActivationFunction
{
    float IActivationFunction.Derivative(float z)
    {
        return 1 - Mathf.Pow(z, 2);
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
