using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeakyReLu : IActivationFunction
{
    private float alpha = 0.01f;

    float IActivationFunction.Func(float z)
    {
        return z > 0 ? z : alpha * z;
    }

    float IActivationFunction.Derivative(float z)
    {
        return z > 0 ? 1 : alpha;
    }

    string IActivationFunction.ToString()
    {
        return "Leaky_ReLu";
    }
}
