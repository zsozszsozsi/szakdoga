using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    public int InCount { get; private set; }
    public int UnitCount { get; private set; }

    public List<Unit> Units { get; private set; }

    public float[] Outputs { get; private set; }
    public float[] OutputSums { get; private set; }


    public Layer(int inCount, int unitCount, IActivationFunction.FunctionType actFunction)
    {
        Units = new List<Unit>();

        this.InCount = inCount;
        this.UnitCount = unitCount;

        Outputs = new float[unitCount];
        OutputSums = new float[unitCount];

        for (int i = 0; i < unitCount; i++)
        {
            IActivationFunction activation = ActivationFunctionFactory.Instance.GetActivation(actFunction);

            Units.Add(new Unit(inCount, activation));
        }
    }

    public void CalcOutputs(params float[] inputs)
    {
        for(int i = 0; i < Units.Count; i++)
        {
            Outputs[i] = Units[i].CalcOutput(inputs);
            OutputSums[i] += Outputs[i];
        }
    }
}
