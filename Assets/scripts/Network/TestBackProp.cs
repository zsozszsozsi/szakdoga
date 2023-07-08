using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBackProp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var network = new NeuralNetwork(3, 
            new int[]{ 4, 2}, 
            new ActivationFunctionType[] { ActivationFunctionType.Sigmoid, ActivationFunctionType.TanH},
            LossType.LogisticLoss);

        
        network.Layers[0].Units[0].Bias = -2;
        network.Layers[0].Units[0].Weights = new float[] { 0.1f, 0.5f, 0.9f };

        network.Layers[0].Units[1].Bias = -6;
        network.Layers[0].Units[1].Weights = new float[] { 0.2f, 0.6f, 1f };

        network.Layers[0].Units[2].Bias = -1;
        network.Layers[0].Units[2].Weights = new float[] { 0.3f, 0.7f, 1.1f };

        network.Layers[0].Units[3].Bias = -7;
        network.Layers[0].Units[3].Weights = new float[] { 0.4f, 0.8f, 1.2f };

        network.Layers[1].Units[0].Bias = -2.5f;
        network.Layers[1].Units[0].Weights = new float[] { 1.3f, 1.5f, 1.7f, 1.9f };

        network.Layers[1].Units[1].Bias = -5;
        network.Layers[1].Units[1].Weights = new float[] { 1.4f, 1.6f, 1.8f, 2 };

        Debug.Log(network);
        //network.FeedForward(1,2,3);
        List<List<float>> samples = new();
        samples.Add(new List<float>() { -0.85f, 0.75f, 1, 2, 3 });
        network.Learn(1, samples);
        Debug.Log(network);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
