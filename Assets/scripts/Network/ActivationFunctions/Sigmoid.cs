using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sigmoid : IActivationFunction
{
    float IActivationFunction.Func(float z)
    {
        return 1 / (1 + Mathf.Exp(-z));
    }

    float IActivationFunction.Derivative(float z)
    {
        return z * (1 - z);
    }

    string IActivationFunction.ToString()
    {
        return "Sigmoid";
    }
}
