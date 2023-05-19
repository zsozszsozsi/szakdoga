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

        Randomize();
    }

    private void Randomize()
    {
        for(int i = 0; i < Weights.Length; i++)
        {
            Weights[i] = Random.value;
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
