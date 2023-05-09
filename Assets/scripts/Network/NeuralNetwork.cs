using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NeuralNetwork
{
    public List<Layer> Layers { get; private set; }
    private ILossFunction LossFunction;
    private System.Random random;

    public NeuralNetwork(int featureCount, int[] layerSizes, IActivationFunction.FunctionType[] actFunctions, ILossFunction.LossType lossFunction)
    {
        random = new System.Random(System.DateTime.Now.Millisecond);

        switch (lossFunction)
        {
            case ILossFunction.LossType.LogisticLoss:
                LossFunction = new LogisticLoss();
                break;
            case ILossFunction.LossType.MSE:
                break;
            default:
                break;
        }

        Layers = new List<Layer>();

        for (int i = 0; i < layerSizes.Length; i++)
        {
            if (i == 0) 
                Layers.Add(new Layer(featureCount, layerSizes[i], actFunctions[i]));
            else
                Layers.Add(new Layer(Layers[^1].UnitCount, layerSizes[i], actFunctions[i]));
        }
    }

    /// <summary>
    /// Calculates the output of the network for the given inputs
    /// </summary>
    /// <param name="inputs">Inputs should be like 0.3, 0.5 here the first is the x coordinate and second is the y coordinate</param>
    /// <returns></returns>
    public float FeedForward(params float[] inputs)
    {
        for(int i = 0; i < Layers.Count; i++)
        {           
            if(i == 0)
            {
                Layers[i].CalcOutputs(inputs);
                continue;
            }
            Layers[i].CalcOutputs(Layers[i-1].Outputs);
            
            
        }

        return Layers[^1].Outputs[0];
    }

    /// <summary>
    /// Calculates the loss for the samples
    /// </summary>
    /// <param name="label"></param>
    /// <param name="inputs"></param>
    /// <returns></returns>
    private float CalculateLoss(List<List<float>> samples)
    {
        float loss = 0f;

        for(int i = 0; i < samples.Count; i++)
        {
            loss += LossFunction.Loss(FeedForward(samples[i].Skip(1).ToArray()), samples[i][0]);
        }

        loss /= samples.Count;
    
        return loss;
    }

    public void Train(List<List<float>> samples, float lr = 0.001f)
    {
        for(int i = 0; i < samples.Count; i++)
        {
            FeedForward(samples[i].Skip(Layers[^1].UnitCount).ToArray());

            //Calculates the output layer gradients
            for(int j = 0; j < Layers[^1].UnitCount; j++)
            {
                var output = Layers[^1].Outputs[j];
                Layers[^1].Units[j].Grad = Layers[^1].Units[j].Activation.Derivative(output) * (samples[i][j] - output);
            }

            //Calculates the hidden layers gradients
            for(int j = Layers.Count-2; j >= 0; j--)
            {
                for(int k = 0; k < Layers[j].UnitCount; k++)
                {
                    var output = Layers[j].Outputs[k];
                    var sums = 0f;
                    for(int l = 0; l < Layers[j + 1].UnitCount; l++)
                    {
                        sums += Layers[j + 1].Units[l].Grad * Layers[j + 1].Units[l].Weights[k];
                    }

                    Layers[j].Units[k].Grad = Layers[j].Units[k].Activation.Derivative(output) * sums;
                }
            }

            //Update weights and biases
            for(int j = 0; j < Layers.Count; j++)
            {
                var layer = Layers[j];

                foreach(var unit in layer.Units)
                {
                    unit.Bias += lr * unit.Grad * 1;

                    for (int k = 0; k < unit.Weights.Length; k++)
                    {
                        if(j == 0)
                        {
                            unit.Weights[k] += lr * unit.Grad * samples[i][k + Layers[^1].UnitCount]; // we need to add the output layers unit count because the first elems in samples are the true labels
                            continue;
                        }

                        unit.Weights[k] += lr * unit.Grad * Layers[j-1].Outputs[k];
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        string s = "";

        for(int i = 0; i < Layers.Count; i++)
        {
            var layer = Layers[i];
            s += $"Layer{i}: (Activation type : {layer.Units[0].Activation})\n";
            for(int j = 0; j < layer.Units.Count; j++)
            {
                var unit = layer.Units[j];
                s += $"Grad: {unit.Grad} Output: {layer.Outputs[j]} \n\tBias: {unit.Bias}, Weigths = [ ";
                foreach(var weight in unit.Weights)
                {
                    s += $"{weight}; ";
                }
                s = $"{s[..^2]} ]\n";
            }
        }

        return s;
    }

    public void Learn(int epochs, List<List<float>> samples, int batchSize = 50)
    {
        //samples = samples.OrderBy(x => random.Next()).Take(Mathf.Min(batchSize, samples.Count)).ToList();

        for (int i= 0; i < epochs; i++)
        {
            Train(samples);
        }

        Debug.Log("Loss after learning: " + CalculateLoss(samples));
        //Debug.Log(this);
    }
}