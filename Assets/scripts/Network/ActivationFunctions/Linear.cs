using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : IActivationFunction
{
    float IActivationFunction.Func(float z)
    {
        return z;
    }

    float IActivationFunction.Derivative(float z)
    {
        return 1;
    }
    string IActivationFunction.ToString()
    {
        return "Linear";
    }
}
