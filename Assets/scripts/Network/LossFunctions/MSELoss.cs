using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSELoss : ILossFunction
{
    public float Loss(float pred, float y)
    {
        return Mathf.Pow(pred - y, 2);
    }
}
