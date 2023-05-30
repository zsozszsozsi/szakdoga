using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public float[] Weights { get; set; }
    public float Bias { get; set; }

    public float Grad;
    public float GradSum;

    public IActivationFunction Activation { get; private set; }

    public Unit(int weightsCount, IActivationFunction activation)
    {
        this.Weights = new float[weightsCount];
        this.Activation = activation;
        this.Grad = 0;
        this.GradSum = 0;

    }

    public void InitWeights(int outCount = 0)
    {
        for (int i = 0; i < Weights.Length; i++)
        {
            var sd = Mathf.Sqrt(2f / (Weights.Length + outCount));
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
