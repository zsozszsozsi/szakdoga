using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public float[] Weights { get; private set; }
    public float Bias { get; private set; }
    public IActivationFunction Activation { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Unit(int weightsCount, IActivationFunction activation)
    {
        this.Weights = new float[weightsCount];
        this.Activation = activation;

    }

    public float CalcOutput(params float[] inputs)
    {
        float output = 0f;
        Weights[1] = 1;
        for (int i = 0; i < inputs.Length; i++)
        {
            output += Weights[i] * inputs[i];
        }

        output += Bias;

        return Activation.Func(output);
    }
}
