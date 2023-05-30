using System.Collections;
using System.Collections.Generic;
using Tensorflow.Keras;
using UnityEngine;
using static UnityEditor.Handles;

public interface IActivationFunction
{
    public enum FunctionType
    {
        Linear,
        Sigmoid,
        ReLu,
        LeakyReLu,
        TanH
    }

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

    public IActivationFunction GetActivation(IActivationFunction.FunctionType type)
    {
        IActivationFunction activation;
        switch (type)
        {
            case IActivationFunction.FunctionType.Sigmoid:
                activation = new Sigmoid();
                break;
            case IActivationFunction.FunctionType.ReLu:
                activation = new ReLu();
                break;
            case IActivationFunction.FunctionType.TanH:
                activation = new TanH();
                break;
            case IActivationFunction.FunctionType.LeakyReLu:
                activation = new LeakyReLu();
                break;
            default:
                activation = new Linear();
                break;
        }

        return activation;
    }
}