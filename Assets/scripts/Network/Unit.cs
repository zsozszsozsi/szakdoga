using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet;
using Tensorflow.Keras.Metrics;

public enum WeightInit
{
    HeInit = 0,
    XavierInit = 1,
    None = 2,
}

public class Unit
{
    public float[] Weights { get; set; }
    public float Bias { get; set; }

    public float Grad;
    public float GradSum;

    public IActivationFunction Activation { get; private set; }

    private MathNet.Numerics.Distributions.Normal normalDist;

    public Unit(int weightsCount, IActivationFunction activation)
    {
        this.Weights = new float[weightsCount];
        this.Activation = activation;
        this.Grad = 0;
        this.GradSum = 0;
    }

    public void InitWeights(WeightInit mode, int outCount = 0)
    {
        switch (mode)
        {
            case WeightInit.HeInit:
                HeInit(outCount);
                break;
            case WeightInit.XavierInit:
                XavierInit();
                break;
            default:
                NoneInit();
                break;

        }
    }

    private void XavierInit()
    {
        var lower = -(1 / Mathf.Sqrt(Weights.Length));
        var upper = (1 / Mathf.Sqrt(Weights.Length));

        for (int i = 0; i < Weights.Length; i++)
        {
            
            Weights[i] = lower + Random.value * (upper - lower);
        }


        Bias = 0f;
    }

    private void NoneInit()
    {
        normalDist = new MathNet.Numerics.Distributions.Normal(0, Mathf.Sqrt(2f / outCount));

        for (int i = 0; i < Weights.Length; i++)
        {

            Weights[i] = Random.value;
        }


        Bias = 0f;
    }

    private void HeInit(int outCount = 0)
    {
        var sd = Mathf.Sqrt(2f / (Weights.Length + outCount));

        for (int i = 0; i < Weights.Length; i++)
        {    
            Weights[i] = Random.value * sd;
        }


        Bias = 0f;
    }

    public float CalcOutput(params float[] inputs)
    {
        float output = 0f;
        for (int i = 0; i < inputs.Length; i++)
        {
            output += Weights[i] * inputs[i];
        }

        output += Bias;
        return Activation.Func(output);
    }
}
