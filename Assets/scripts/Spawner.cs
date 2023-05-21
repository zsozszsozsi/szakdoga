using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [Range(0.1f, 0.5f)]
    public float Step;

    public GameObject RedSample;
    public GameObject BlueSample;

    public Button Btn1;
    public Button Btn2;
    public Button Btn3;
    public Button Btn4;

    private Simulator Simulator;

    public void Start()
    {
        Simulator = Simulator.Instance;
    }

    public void Update()
    {
        if (Simulator.IsLearning)
        {
            Btn1.interactable = false;
            Btn2.interactable = false;
            Btn3.interactable = false;
            Btn4.interactable = false;
        }
        else
        {
            Btn1.interactable = true;
            Btn2.interactable = true;
            Btn3.interactable = true;
            Btn4.interactable = true;
        }
    }

    public void Pattern1()
    {
        for(float i = Simulator.MinX; i < 0f; i += Step)
        {
            for(float j = Simulator.MinY; j < 0f; j += Step)
            {
                Simulator.InstantiateSample(RedSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f));
            }
        }

        for (float i = 0; i < Simulator.MaxX; i += Step)
        {
            for (float j = 0; j < Simulator.MaxY; j += Step)
            {
                Simulator.InstantiateSample(BlueSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f));
            }
        }
    }

    public void Pattern2()
    {

    }

    public void Pattern3()
    {

    }

    public void Pattern4()
    {

    }
}
