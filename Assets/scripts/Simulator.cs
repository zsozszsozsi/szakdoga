using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Simulator : MonoBehaviour
{
    public static Simulator Instance;

    [HideInInspector]
    public bool IsLearning = false;

    private int BlueCount;
    private int RedCount;

    private List<List<float>> SamplesForNetwork;

    public GameObject Spawner;
    public GameObject Metrics;
    public GameObject Settings;
    public GameObject Buttons;

    public Slider W0;
    public Slider W1;
    public Slider B0;

    public Button LearnBtn;
    public Button ClearBtn;

    public GameObject DecisionBoundaryGO;
    private DecisionBoundary DecisionBoundary;

    public float MaxX;
    public float MinX;

    public float MaxY;
    public float MinY;

    private float[] weigths = new float[2];
    private float[] biases = new float[1];

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
        SamplesForNetwork = new List<List<float>>();

        weigths[0] = W0.value;
        weigths[1] = W1.value;
        biases[0] = B0.value;

        if (Network.Instance == null)
        {
            network = new NeuralNetwork(2, new int[] { 1 }, new ActivationFunctionType[] { ActivationFunctionType.Sigmoid }, LossType.LogisticLoss);
        }
        else
        {
            network = Network.Instance.NeuralNetwork;
        }

        if(network.Layers.Count != 1 || network.Layers[0].UnitCount != 1)
        {
            Settings.SetActive(false);
            Buttons.transform.localPosition = new Vector3(0, 165f, 0);
        }

        DecisionBoundary.DrawDecisionBoundaryWithText(network);

        Metrics.GetComponent<TextMeshProUGUI>().text = $"Iterations: _\tLoss: _";
    }


    [Range(0, 1)] public float cd = 0.5f;
    [Range(0.00001f, 1)] public float lr = 0.001f;
    private float t = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!IsLearning)
        {
            ClearBtn.interactable = true;

            W0.interactable = true;

            W1.interactable = true;

            B0.interactable = true;
            
            network.SetWeight(W0.value, W1.value, B0.value);
            var loss = SamplesForNetwork.Any() ? network.CalculateLoss(SamplesForNetwork).ToString() : "";
            Metrics.GetComponent<TextMeshProUGUI>().text = $"Iterations: 0\tLoss: {loss:0.00000}";
            DecisionBoundary.DrawDecisionBoundaryWithText(network);
            
        }
        else
        {
            ClearBtn.interactable = false;

            W0.interactable = false;
            W1.interactable = false;
            B0.interactable = false;

            W0.value = network.Layers[0].Units[0].Weights[0];
            W1.value = network.Layers[0].Units[0].Weights[1];
            B0.value = network.Layers[0].Units[0].Bias;

        }

        if (RedCount > 0 && BlueCount > 0)
            LearnBtn.interactable = true;
        else
            LearnBtn.interactable = false;
        
        t += Time.deltaTime;

        if (IsLearning && t > cd)
        {
            network.Learn(50, SamplesForNetwork);
            DecisionBoundary.DrawDecisionBoundaryWithText(network);
            Metrics.GetComponent<TextMeshProUGUI>().text = $"Iterations: {network.Iterations}\tLoss: {network.Loss:0.00000}";
            
            t = 0f;
        }
    }

    public bool InstantiateSample(GameObject sample, Vector3 pos)
    {
        if(Spawner.transform.childCount > 500)
        {
            ErrorManager.Instance.AddError("Can't spawn more than 500 samples!");
            return false;
        }

        var go = Instantiate(sample, pos, Quaternion.identity, Spawner.transform);
        var label = go.GetComponent<LabelManager>().Label;
        SamplesForNetwork.Add(new List<float> { label, go.transform.position.x, go.transform.position.y });
        
        if(label == 1)
        {
            RedCount++;
        }
        else
        {
            BlueCount++;
        }

        return true;
    }

    public void ClearSamples()
    {
        for(int i = 0; i < Spawner.transform.childCount; i++)
        {
            Destroy(Spawner.transform.GetChild(i).gameObject);
        }

        SamplesForNetwork = new List<List<float>>();

        RedCount = 0;
        BlueCount = 0;

        network.Iterations = 0;
      
    }

    public void BackToBuilder()
    {
        SceneManager.LoadScene("NetworkBuilder");
    }

    public void Learn()
    {
        IsLearning = !IsLearning;
        LearnBtn.GetComponentInChildren<Text>().text = IsLearning ? "Stop Learning!": "Start Learning!";
        //SamplesForNetwork = Network.Instance.NeuralNetwork.Standardization(SamplesForNetwork);
    }
}
