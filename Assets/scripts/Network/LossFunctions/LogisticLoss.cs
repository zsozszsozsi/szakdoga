using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogisticLoss : ILossFunction
{
    private float log(float x)
    {
        if (x != 0)
        {
            return Mathf.Log(x);
        }

        return float.MinValue;
    }

    public float Loss(float pred, float y)
    {
        return -y * log(pred) - (1 - y) * log(1 - pred);
    }
}
