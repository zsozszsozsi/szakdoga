using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    public float Step;

    public GameObject RedSample;
    public GameObject BlueSample;

    public Transform SampleParent;

    public Button Btn1;
    public Button Btn2;
    public Button Btn3;
    public Button Btn4;

    private LogRegression LogReg;

    public void Start()
    {
        LogReg = transform.GetComponent<LogRegression>();
    }

    public void Update()
    {
        if (LogReg.IsLearning)
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
        for(float i = MinX; i < 0f; i += Step)
        {
            for(float j = MinY; j < 0f; j += Step)
            {
                LogReg.InstantiateSample(RedSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f));
                //Instantiate(RedSample, new Vector3(i + Random.Range(0,0.5f), j + Random.Range(0, 0.5f), -1f), Quaternion.identity, SampleParent);
            }
        }

        for (float i = 0; i < MaxX; i += Step)
        {
            for (float j = 0; j < MaxY; j += Step)
            {
                LogReg.InstantiateSample(BlueSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f));
                //Instantiate(BlueSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f), Quaternion.identity, SampleParent);
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
