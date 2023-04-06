using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivationFunction
{
    public enum FunctionType
    {
        Linear,
        Sigmoid,
        ReLu
    }

    public float Func(float z);
}
