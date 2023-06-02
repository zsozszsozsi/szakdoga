using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Simulator : MonoBehaviour
{
    public static Simulator Instance;

    [HideInInspector]
    public bool IsLearning = false;

    private int BlueCount;
    private int RedCount;

    private List<GameObject> Samples;
    private List<List<float>> SamplesForNetwork;

    public GameObject Spawner;
    public GameObject BlueSample;
    public GameObject RedSample;

    public Slider W0;
    public Slider W1;
    public Slider B0;

    public Button LearnBtn;
    public Button ClearBtn;

    public GameObject DecisionBoundaryGO;
    private DecisionBoundary DecisionBoundary;

    public float MaxX = 2.6f;
    public float MinX = -2.6f;

    public float MaxY = 1.27f;
    public float MinY = -1.27f;

    private float[] weigths = new float[2];
    private float[] biases = new float[1];

    public float b_0;
    public float w_0;
    public float w_1;

    private NeuralNetwork network;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DecisionBoundary = DecisionBoundaryGO.GetComponent<DecisionBoundary>();
        Samples = new List<GameObject>();
        SamplesForNetwork = new List<List<float>>();

        weigths[0] = W0.value;
        weigths[1] = W1.value;
        biases[0] = B0.value;


        network = new NeuralNetwork(2, new int[] { 1 }, new IActivationFunction.FunctionType[] { IActivationFunction.FunctionType.Sigmoid }, ILossFunction.LossType.LogisticLoss);
        //Debug.Log(network);
        //DecisionBoundary.DrawDecisionBoundary(network);
        DecisionBoundary.DrawDecisionBoundaryWithText(Network.Instance.NeuralNetwork);
    }


    [Range(0, 1)] public float cd = 0.5f;
    [Range(0.00001f, 1)] public float lr = 0.001f;
    private float t = 0f;

    // Update is called once per frame
    void Update()
    {
        w_0 = weigths[0];
        w_1 = weigths[1];
        b_0 = biases[0];

        if (!IsLearning)
        {
            ClearBtn.interactable = true;

            W0.interactable = true;
            weigths[0] = W0.value;

            W1.interactable = true;
            weigths[1] = W1.value;

            B0.interactable = true;
            biases[0] = B0.value;
        }
        else
        {
            ClearBtn.interactable = false;

            W0.interactable = false;
            W1.interactable = false;
            B0.interactable = false;
        }

        if(RedCount > 0 && BlueCount > 0)
            LearnBtn.interactable = true;
        else
            LearnBtn.interactable = false;

        //DecisionBoundary.DrawDecisionBoundary(weigths[0], weigths[1], biases[0]);
        //DecisionBoundary.DrawDecisionBoundary(network.Layers[0].Units[0].Weights[0], network.Layers[0].Units[0].Weights[1], network.Layers[0].Units[0].Bias);
        //DecisionBoundary.DrawDecisionBoundary(network);
        //DecisionBoundary.DrawDecisionBoundary(Network.Instance.NeuralNetwork.Layers[0].Units[0].Weights[0], Network.Instance.NeuralNetwork.Layers[0].Units[0].Weights[1], Network.Instance.NeuralNetwork.Layers[0].Units[0].Bias);

        // Left Click: spawn red sample
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.point.x < MaxX && hit.point.x > MinX && hit.point.y < MaxY && hit.point.y > MinY)
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -1);

                    var go = Instantiate(RedSample, hit.point, Quaternion.identity, Spawner.transform);

                    RedCount++;
                    Samples.Add(go);
                    SamplesForNetwork.Add(new List<float> {go.GetComponent<LabelManager>().Label, go.transform.position.x, go.transform.position.y});
                }
            }

        }

        // Right Click: spawn blue sample
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.point.x < MaxX && hit.point.x > MinX && hit.point.y < MaxY && hit.point.y > MinY)
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -1);

                    var go = Instantiate(BlueSample, hit.point, Quaternion.identity, Spawner.transform);

                    BlueCount++;
                    Samples.Add(go);
                    SamplesForNetwork.Add(new List<float> { go.GetComponent<LabelManager>().Label, go.transform.position.x, go.transform.position.y });
                }
                
            }
        }
        
        t += Time.deltaTime;

        if (IsLearning && t > cd)
        {
            //GradientDescent(100, lr);
            //network.Learn(100, SamplesForNetwork);
            //DecisionBoundary.DrawDecisionBoundary(network);
            
            if(Network.Instance.NeuralNetwork != null)
            {
                Network.Instance.NeuralNetwork.Learn(50, SamplesForNetwork);
                DecisionBoundary.DrawDecisionBoundaryWithText(Network.Instance.NeuralNetwork);
            }
            else
            {
                print("First you need to build a network!");
            }
            
            t = 0f;
        }
    }

    public void InstantiateSample(GameObject sample, Vector3 pos)
    {
        var go = Instantiate(sample, pos, Quaternion.identity, Spawner.transform);
        var label = go.GetComponent<LabelManager>().Label;
        SamplesForNetwork.Add(new List<float> { label, go.transform.position.x, go.transform.position.y });
        Samples.Add(go);
        
        if(label == 1)
        {
            RedCount++;
        }
        else
        {
            BlueCount++;
        }
    }

    public void ClearSamples()
    {
        
        for(int i = 0; i < Samples.Count; i++)
        {
            Destroy(Samples[i]);
        }

        Samples = new List<GameObject>();
        SamplesForNetwork = new List<List<float>>();

        RedCount = 0;
        BlueCount = 0;
    }

    public void Learn()
    {
        IsLearning = !IsLearning;
        LearnBtn.GetComponentInChildren<Text>().text = IsLearning ? "Stop Learning!": "Start Learning!";
        //SamplesForNetwork = Network.Instance.NeuralNetwork.Standardization(SamplesForNetwork);
    }

    private float Sigmoid(float x)
    {
        return 1 / ( 1 + Mathf.Exp(-x) );
    }

    private float ComputeOutput(Transform sample)
    {
        return Sigmoid(
            weigths[0] * sample.position.x
            + weigths[1] * sample.position.y
            + biases[0]
            );
    }

    private float log(float x)
    {
        if (x != 0)
        {
            return Mathf.Log(x);
        }

        return float.MinValue;
    }

    public float Loss()
    {
        var loss = 0f;

        for (int i = 0; i < Samples.Count; i++)
        {
            var y = Samples[i].transform.GetComponent<LabelManager>().Label; // true label

            var output = ComputeOutput(Samples[i].transform);

            loss += -y * log(output) - ( 1 - y ) * log(1 - output);
        }

        // Loss func is 1/N * (-y * log(y^) - (1-y) * log(1-y^))

        return loss/Samples.Count;
    }


    private void GradientDescent(int epochs, float lr)
    {
        System.Random random = new System.Random();
        
        for(int i = 0; i < epochs; i++)
        {
            Samples = Samples.OrderBy(x => random.Next()).ToList().Take(50).ToList();

            var errors = new float[Samples.Count];

            for(int j = 0; j < Samples.Count; j++)
            {
                var y_true = Samples[j].GetComponent<LabelManager>().Label;
                var y_pred = ComputeOutput(Samples[j].transform);
                errors[j] = y_pred - y_true;

                //Debug.Log(errors[j] + ": " + (y_true == 0 ? "Blue" : "Red"));
            }

            for(int j = 0; j < weigths.Length; j++)
            {
                float gradient = 0f;

                for(int k = 0; k < Samples.Count; k++)
                {
                    if (j == 0)
                        gradient += errors[k] * Samples[k].transform.position.x;
                    else
                        gradient += errors[k] * Samples[k].transform.position.y;
                }

                weigths[j] -= lr * gradient / Samples.Count; 
            }

            for(int j = 0; j < biases.Length; j++)
            {
                float gradient = 0f;
                for(int k = 0; k < Samples.Count; k++)
                {
                    gradient += errors[k] * 1;
                }

                biases[j] -= lr * gradient / Samples.Count;
            }

            W0.value = weigths[0];
            W1.value = weigths[1];
            B0.value = biases[0];
        }

        Debug.Log("Loss: " + Loss());
    }
}
