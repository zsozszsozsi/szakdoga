using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{

    public static Controller Instance;

    public GameObject NeuralNetwork;
    public GameObject Connections;

    public GameObject Input;
    public GameObject Neuron;
    public GameObject Weight;

    public GameObject LayerBtn;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void BuildConnections()
    {
        for(int i = 0; i < Connections.transform.childCount; i++)
        {
            Destroy(Connections.transform.GetChild(i).gameObject);
        }


        for (int i = 0; i < NeuralNetwork.transform.childCount - 1; i++)
        {
            for (int j = 0; j < NeuralNetwork.transform.GetChild(i).childCount; j++) 
            {
                if (NeuralNetwork.transform.GetChild(i).GetChild(j).tag == "Button") continue;

                var from = NeuralNetwork.transform.GetChild(i).GetChild(j).position;

                for (int k = 0; k < NeuralNetwork.transform.GetChild(i + 1).childCount; k++)
                {
                    if (NeuralNetwork.transform.GetChild(i + 1).GetChild(k).tag == "Button") continue;

                    var to = NeuralNetwork.transform.GetChild(i + 1).GetChild(k).position;

                    var wx = (from.x + to.x) / 2;
                    var wy = (from.y + to.y) / 2;

                    var a = to.x - from.x;
                    var b = to.y - from.y;
                    var c = Mathf.Sqrt(a * a + b * b);

                    var alpha = Mathf.Rad2Deg * Mathf.Asin(b / c);

                    var weight = Instantiate(Weight, new Vector3(wx, wy, 7), Quaternion.Euler(0, 0, alpha), Connections.transform);
                    weight.transform.localScale = new Vector3(c, 0.03f, 0.01f);
                    weight.name = $"w{i}{k}{j}";
                    // {i} = i-th layer, k-th neuron, j-th weight

                }
            }
        }
    }

    public void AddInput()
    {
        if(NeuralNetwork.transform.childCount == 0)
        {
            var inputs = new GameObject("Inputs");
            inputs.transform.position = new Vector3(0, 0, 0);
            inputs.transform.parent = NeuralNetwork.transform;
        }

        var inputParent = NeuralNetwork.transform.GetChild(0);
        var childCount = inputParent.childCount;

        if(childCount == 4)
        {
            print("maximum number of inputs is 4!");
            return;
        }

        if (childCount == 0)
        {
            //if there is no input
            Instantiate(Input, inputParent);
        }
        else
        {
            var lastInput = inputParent.GetChild(childCount - 1).position;

            Instantiate(Input, lastInput, Quaternion.identity, inputParent);
            // childCount = inputParent.childCount; here if we dont update we dont need to care about the loop and changing the last pos
            // cause we want to change the pos from 0..childcount-1

            for (int i = 0; i < childCount; i++)
            {
                var input = inputParent.GetChild(i);

                input.position += new Vector3(0, 0.5f, 0);
            }

            inputParent.GetChild(childCount).position += new Vector3(0, -0.5f, 0);
        }

        BuildConnections();
    }
    
    public void RemoveInput()
    {
        var childCount = NeuralNetwork.transform.childCount;
        if(childCount == 0)
        {
            print("Nothing to remove!");
            return;
        }

        var inputParent = NeuralNetwork.transform.GetChild(0);
        var inputCount = inputParent.childCount;
        

        if(inputCount > 1)
        {
            DestroyImmediate(inputParent.GetChild(inputCount - 1).gameObject);

            inputCount = inputParent.childCount;
            for(int i = 0; i < inputCount; i++)
            {
                var input = inputParent.GetChild(i);

                input.position += new Vector3(0, -0.5f, 0);
            }

        }
        else if(inputCount == 1 && childCount == 1) 
        {
            DestroyImmediate(NeuralNetwork.transform.GetChild(0).gameObject);
        }

        BuildConnections();
    }

    public void AddLayer()
    {
        var parent = NeuralNetwork.transform;
        var childCount = parent.childCount;

        if(childCount == 0)
        {
            print("first you need an input layer!");
            return;
        }

        if(childCount == 7)
        {
            print("Maximum amount of layers is 7");
            return;
        }
        
        var lastChild = NeuralNetwork.transform.GetChild(childCount - 1);
        var newLayerGO = new GameObject($"Layer{childCount - 1}");
        Instantiate(LayerBtn, newLayerGO.transform);
        Instantiate(Neuron, newLayerGO.transform);
        newLayerGO.transform.parent = parent;
        newLayerGO.transform.position = lastChild.position;

        for (int i= 0; i<childCount; i++)
        {
            var layer = parent.GetChild(i);
            layer.position += new Vector3(-1, 0, 0);
        }

        newLayerGO.transform.position += new Vector3(1, 0, 0);

        BuildConnections();
    }

    public void RemoveLayer()
    {
        var parent = NeuralNetwork.transform;
        var childCount = parent.childCount;

        if (childCount != 0)
        {
            DestroyImmediate(parent.GetChild(childCount - 1).gameObject);
            childCount = parent.childCount;

            for(int i = 0; i < childCount; i++)
            {
                parent.GetChild(i).transform.position += new Vector3(1, 0, 0);
            }

        }

        BuildConnections();
    }

    public void AddNeuron(Transform layer)
    {
        var childCount = layer.childCount;
        var lastInput = layer.GetChild(childCount - 1).position;

        Instantiate(Neuron, lastInput, Quaternion.identity, layer);
        // childCount = inputParent.childCount; here if we dont update we dont need to care about the loop and changing the last pos
        // cause we want to change the pos from 0..childcount-1

        for (int i = 0; i < childCount; i++)
        {
            var input = layer.GetChild(i);

            if (input.tag == "Button") continue;

            input.position += new Vector3(0, 0.5f, 0);
        }

        layer.GetChild(childCount).position += new Vector3(0, -0.5f, 0);

        BuildConnections();
    }

    public void RemoveNeuron(Transform layer)
    {
        var childCount = layer.childCount;

        if(childCount == 2) // 2 because first is the button and the second one is the one neuron
        {
            print("Use the remove layer button!");
            return;
        }

        DestroyImmediate(layer.GetChild(childCount - 1).gameObject);

        childCount = layer.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var input = layer.GetChild(i);

            if (input.tag == "Button") continue;

            input.position += new Vector3(0, -0.5f, 0);
        }

        BuildConnections();
    }

    public void BuildNetwork()
    {
        var childCount = NeuralNetwork.transform.childCount;
        if(childCount < 2)
        {
            print("First you need to build a network!");
            return;
        }

        var featureCount = NeuralNetwork.transform.GetChild(0).childCount;
        int[] layerSizes = new int[NeuralNetwork.transform.childCount - 1];
        IActivationFunction.FunctionType[] actFunctions = new IActivationFunction.FunctionType[layerSizes.Length];

        for(int i = 1; i < NeuralNetwork.transform.childCount; i++)
        {
            layerSizes[i - 1] = NeuralNetwork.transform.GetChild(i).childCount - 1;
            actFunctions[i - 1] = IActivationFunction.FunctionType.ReLu;
        }
        actFunctions[^1] = IActivationFunction.FunctionType.Sigmoid;

        Network.Instance.NeuralNetwork = 
            new NeuralNetwork(
                featureCount: featureCount,
                layerSizes: layerSizes, 
                actFunctions: actFunctions, 
                lossFunction: ILossFunction.LossType.LogisticLoss
            );

        print(Network.Instance.NeuralNetwork);

        SceneManager.LoadScene("Simulator");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
