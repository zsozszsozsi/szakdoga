using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    public static Network Instance;
    public NeuralNetwork NeuralNetwork;

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }
}
