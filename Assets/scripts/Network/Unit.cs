using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public float[] Weights { get; set; }
    public float Bias { get; set; }

    public float PrevGradient;
    public float Grad;

    public IActivationFunction Activation { get; private set; }

    public Unit(int weightsCount, IActivationFunction activation)
    {
        this.Weights = new float[weightsCount];
        this.Activation = activation;
        this.Grad = 0;
        this.PrevGradient = 0;

    }

    public void InitWeights(int outCount)
    {
        if(Activation is ReLu or Linear)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.value * Mathf.Sqrt(2f / Weights.Length);
            }
        }
        if (Activation is Sigmoid or TanH)
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = Random.value * Mathf.Sqrt(6f / (Weights.Length + outCount));
            }
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
