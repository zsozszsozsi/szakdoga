using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    private List<Layer> Layers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public NeuralNetwork(int featureCount, int[] layerSizes, IActivationFunction.FunctionType[] actFunctions)
    {
        Layers = new List<Layer>();

        for (int i = 0; i < layerSizes.Length; i++)
        {
            if (i == 0) 
                Layers.Add(new Layer(featureCount, layerSizes[i], actFunctions[i]));
            else
                Layers.Add(new Layer(Layers[Layers.Count-1].UnitCount, layerSizes[i], actFunctions[i]));
        }
    }

    public float FeedForward(params float[] inputs)
    {
        for(int i = 0; i < Layers.Count; i++)
        {           
            if(i == 0)
            {
                Layers[i].CalcOutputs(inputs);
            }
            else
            {
                Layers[i].CalcOutputs(Layers[i-1].Outputs);
            }
            
        }

        return Layers[Layers.Count-1].Outputs[0];
    }

    private void Learn()
    {

    }
}
