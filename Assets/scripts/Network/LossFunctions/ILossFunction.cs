using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILossFunction
{
    public enum LossType
    {
        LogisticLoss,
        MSE
    }

    public float Loss(float pred, float y);
}
