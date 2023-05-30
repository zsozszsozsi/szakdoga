using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReLu : IActivationFunction
{

    float IActivationFunction.Func(float z)
    {
        return z > 0 ? z : 0;
    }

    float IActivationFunction.Derivative(float z)
    {
        return z > 0 ? 1 : 0;
    }

    string IActivationFunction.ToString()
    {
        return "ReLu";
    }
}
