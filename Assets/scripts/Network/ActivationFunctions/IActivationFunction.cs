using System.Collections;
using System.Collections.Generic;
using Tensorflow.Keras;
using UnityEngine;
using static UnityEditor.Handles;

public enum ActivationFunctionType
{
    Linear,
    Sigmoid,
    ReLu,
    LeakyReLu,
    TanH
}

public interface IActivationFunction
{
    public float Func(float z);

    public float Derivative(float z);

    public string ToString();
}

public class ActivationFunctionFactory
{
    private static ActivationFunctionFactory instance = null;

    public ActivationFunctionFactory() { }

    public static ActivationFunctionFactory Instance
    {
        get 
        { 
            if(instance == null)
            {
                instance = new ActivationFunctionFactory();
            }

            return instance;
        }
    }

    public IActivationFunction GetActivation(ActivationFunctionType type)
    {
        IActivationFunction activation;
        switch (type)
        {
            case ActivationFunctionType.Sigmoid:
                activation = new Sigmoid();
                break;
            case ActivationFunctionType.ReLu:
                activation = new ReLu();
                break;
            case ActivationFunctionType.TanH:
                activation = new TanH();
                break;
            case ActivationFunctionType.LeakyReLu:
                activation = new LeakyReLu();
                break;
            default:
                activation = new Linear();
                throw new System.NotImplementedException("Choosed activation not implemented yet, using Linear function instead.");
        }

        return activation;
    }
}