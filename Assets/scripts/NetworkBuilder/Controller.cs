using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public static Controller Instance;

    public GameObject NeuralNetwork;
    public GameObject Connections;

    public GameObject Input;
    public GameObject Neuron;
    public GameObject Weight;

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
        for (int i = 0; i < NeuralNetwork.transform.childCount - 1; i++)
        {
            for (int j = 0; j < NeuralNetwork.transform.GetChild(i).childCount; j++)
            {
                var from = NeuralNetwork.transform.GetChild(i).GetChild(j).position;

                for (int k = 0; k < NeuralNetwork.transform.GetChild(i + 1).childCount; k++)
                {
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
            Instantiate(Input, new Vector3(0,0,7), Quaternion.identity, inputParent);
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
    }
    
    public void RemoveInput()
    {
        var inputParent = NeuralNetwork.transform.GetChild(0);
        var childCount = inputParent.childCount;

        if(childCount != 0)
        {
            Destroy(inputParent.GetChild(childCount - 1).gameObject);

            childCount = inputParent.childCount;
            for(int i = 0; i < childCount; i++)
            {
                var input = inputParent.GetChild(i);

                input.position += new Vector3(0, -0.5f, 0);
            }

        }
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

        if(childCount == 8)
        {
            print("Maximum amount of layers is 7");
            return;
        }
        
        var lastChild = NeuralNetwork.transform.GetChild(childCount - 1);
        var newLayerGO = new GameObject($"Layer{childCount - 1}");
        Instantiate(Neuron, new Vector3(0, 0, 7), Quaternion.identity, newLayerGO.transform);
        newLayerGO.transform.parent = parent;
        newLayerGO.transform.position = lastChild.position;

        for (int i= 0; i<childCount; i++)
        {
            var layer = parent.GetChild(i);
            layer.position += new Vector3(-1, 0, 0);
        }

        newLayerGO.transform.position += new Vector3(1, 0, 0);
    }

    public void RemoveLayer()
    {
        var parent = NeuralNetwork.transform;
        var childCount = parent.childCount;

        if (childCount != 0)
        {
            Destroy(parent.GetChild(childCount - 1).gameObject);
            childCount = parent.childCount;

            for(int i = 0; i < childCount; i++)
            {
                parent.GetChild(i).transform.position += new Vector3(1, 0, 0);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        BuildConnections();
    }
}
