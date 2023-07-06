using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LossType
{
    LogisticLoss,
    MSE
}


public interface ILossFunction
{ 
    public float Loss(float pred, float y);
}

public class LossFunctionFactory
{
    private static LossFunctionFactory instance = null;

    public LossFunctionFactory() { }

    public static LossFunctionFactory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LossFunctionFactory();
            }

            return instance;
        }
    }

    public ILossFunction GetLossFunction(LossType type)
    {
        ILossFunction activation;
        switch (type)
        {
            case LossType.LogisticLoss:
                activation = new LogisticLoss();
                break;
            case LossType.MSE:
                activation = new MSELoss();
                break;
            default:
                activation = new MSELoss();
                throw new System.NotImplementedException("Choosed loss function not implemented yet, using MSE loss function instead.");
        }

        return activation;
    }
}