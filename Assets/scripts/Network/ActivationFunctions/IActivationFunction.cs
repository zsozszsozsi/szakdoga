using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivationFunction
{
    public enum FunctionType
    {
        Linear,
        Sigmoid,
        ReLu,
        TanH
    }

    public float Func(float z);

    public float Derivative(float z);

    public string ToString();
}
