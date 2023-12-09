using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NeuralNetwork
{
    public List<Layer> Layers { get; private set; }
    public int Iterations = 0;
    public float Loss = 100f;

    private ILossFunction LossFunction;
    private System.Random random;
    private List<float> Losses;
    private int FeatureCount;
    private int LabelCount;

    public NeuralNetwork(int featureCount, int[] layerSizes, ActivationFunctionType[] actFunctions, LossType lossFunction)
    {
        FeatureCount = featureCount;
        Losses = new List<float>();

        random = new System.Random(System.DateTime.Now.Millisecond);

        try
        {
            LossFunction = LossFunctionFactory.Instance.GetLossFunction(lossFunction);
        }catch(System.NotImplementedException ex)
        {
            Debug.LogError(ex.Message);
        }

        InitializeLayers(featureCount, layerSizes, actFunctions);

        InitializeWeights();

        LabelCount = Layers[^1].UnitCount;
    }

    public void SetWeight(float w0, float w1, float b)
    {
        if (Layers.Count != 1 || Layers[0].UnitCount != 1)
            return;

        Layers[0].Units[0].Weights[0] = w0;
        Layers[0].Units[0].Weights[1] = w1;
        Layers[0].Units[0].Bias = b;
    }

    private void InitializeLayers(int featureCount, int[] layerSizes, ActivationFunctionType[] actFunctions)
    {
        Layers = new List<Layer>();

        for (int i = 0; i < layerSizes.Length; i++)
        {
            if (i == 0)
                Layers.Add(new Layer(featureCount, layerSizes[i], actFunctions[i]));
            else
                Layers.Add(new Layer(Layers[^1].UnitCount, layerSizes[i], actFunctions[i]));
        }
    }

    private void InitializeWeights()
    {
        for(int i = 0; i < Layers.Count; i++)
        {
            foreach (var unit in Layers[i].Units)
            {
                if(i+1 == Layers.Count)
                {
                    unit.InitWeights(); // this is the last layer
                    continue;
                }
                unit.InitWeights(Layers[i + 1].UnitCount);
            }
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
    public float CalculateLoss(List<List<float>> samples)
    {
        float loss = 0f;

        for(int i = 0; i < samples.Count; i++)
        {
            loss += LossFunction.Loss(FeedForward(samples[i].Skip(1).ToArray()), samples[i][0]);
        }

        loss /= samples.Count;
    
        return loss;
    }

    public void Train(List<List<float>> samples, float lr = 0.001f, float momentum = 0.4f)
    {
        List<float> inputSums = new(new float[samples[0].Count - Layers[^1].UnitCount]); // samples[0]: {0, 12, 32} -> {label, x, y} -> label is as long as the last layer unit count

        for(int i = 0; i < samples.Count; i++)
        {
            FeedForward(samples[i].Skip(Layers[^1].UnitCount).ToArray());

            //Calculates the output layer gradients
            for(int j = 0; j < Layers[^1].UnitCount; j++)
            {
                var output = Layers[^1].Outputs[j];
                //Layers[^1].Units[j].Grad = Layers[^1].Units[j].Activation.Derivative(output) * LossFunction.Loss(output, samples[i][j]);
                Layers[^1].Units[j].Grad = Layers[^1].Units[j].Activation.Derivative(output) * (samples[i][j] - output);
                //Layers[^1].Units[j].GradSum += Layers[^1].Units[j].Grad;
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
                    //Layers[j].Units[k].GradSum += Layers[j].Units[k].Grad;

                }
            }

            /*foreach(var unit in Layers[0].Units)
            {
                for(int j = 0; j < unit.Weights.Length; j++)
                {
                    inputSums[j] += samples[i][j + Layers[^1].UnitCount];
                }
            }*/

            for (int j = 0; j < Layers.Count; j++)
            {
                var layer = Layers[j];

                foreach (var unit in layer.Units)
                {
                    unit.Bias += lr * unit.Grad * 1;

                    for (int k = 0; k < unit.Weights.Length; k++)
                    {
                        if (j == 0)
                        {
                            // the first layes has feature count weights
                            unit.Weights[k] += lr * unit.Grad * samples[i][k + Layers[^1].UnitCount]; // we need to add the output layers unit count because the first elems in samples are the true labels
                            continue;
                        }

                        unit.Weights[k] += lr * unit.Grad * Layers[j - 1].Outputs[k];
                    }


                }
            }
        }

        /*//Update weights and biases
        for (int j = 0; j < Layers.Count; j++)
        {
            var layer = Layers[j];

            foreach (var unit in layer.Units)
            {
                unit.Bias -= lr * (unit.GradSum / samples.Count) * 1;

                for (int k = 0; k < unit.Weights.Length; k++)
                {
                    if (j == 0)
                    {
                        // the first layes has feature count weights
                        unit.Weights[k] -= lr * (unit.GradSum / samples.Count) * (inputSums[k] / samples.Count); // we need to add the output layers unit count because the first elems in samples are the true labels
                        continue;
                    }

                    unit.Weights[k] -= lr * (unit.GradSum / samples.Count) * (Layers[j - 1].OutputSums[k] / samples.Count);
                }

                
            }
        }*/

    }

    public override string ToString()
    {
        string s = "";

        for(int i = 0; i < Layers.Count; i++)
        {
            var layer = Layers[i];
            s += $"Layer{i}: (Activation type : {layer.Units[0].Activation})\n";
            
            s += $"\t Outputs = [ ";
            foreach (var output in layer.Outputs)
            {
                s += $"{output}; ";
            }
            s = $"{s[..^2]} ]\n";

            for (int j = 0; j < layer.Units.Count; j++)
            {
                var unit = layer.Units[j];
                s += $"\tGrad: {unit.Grad}";

                s += $"\tBias: {unit.Bias}, Weigths = [ ";
                foreach (var weight in unit.Weights)
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

        //samples = Standardization(samples);

        for (int i= 0; i < epochs; i++)
        {
            Train(samples);
        }

        var loss = CalculateLoss(samples);
        Losses.Add(loss);

        Iterations += epochs;
        Loss = loss;
    }

    public List<List<float>> Standardization(List<List<float>> samples)
    {
        List<float> means = GetMeans(samples);
        List<float> devs = GetDeviation(samples, means);

        foreach(var sample in samples)
        {
            for(int i = LabelCount; i < sample.Count; i++)
            {
                sample[i] = (sample[i] - means[i - LabelCount]) / devs[i - LabelCount];
            }
        }
        

        return samples;
    }

    private List<float> GetMeans(List<List<float>> samples)
    {
        List<float> means = new(new float[FeatureCount]);

        for (int i = 0; i < samples.Count; i++)
        {
            for (int j = LabelCount; j < samples[i].Count; j++)
            {
                means[j - LabelCount] += samples[i][j];
            }
        }

        for (int i = 0; i < means.Count; i++)
        {
            means[i] /= samples.Count;
        }

        return means;
    }

    private List<float> GetDeviation(List<List<float>> samples, List<float> means)
    {
        List<float> devs = new(new float[FeatureCount]);

        for (int i = 0; i < samples.Count; i++)
        {
            for (int j = LabelCount; j < samples[i].Count; j++)
            {
                devs[j - LabelCount] += Mathf.Pow(samples[i][j] - means[j - LabelCount], 2);
            }
        }

        for (int i = 0; i < means.Count; i++)
        {
            devs[i] /= (samples.Count - 1);
            devs[i] = Mathf.Sqrt(devs[i]);
        }

        return devs;
    }
}
