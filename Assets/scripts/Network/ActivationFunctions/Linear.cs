using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : IActivationFunction
{
    float IActivationFunction.Func(float z)
    {
        return z;
    }
}
