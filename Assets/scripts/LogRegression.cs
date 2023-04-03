using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class LogRegression : MonoBehaviour
{
    public GameObject BlueParent;
    public GameObject RedParent;

    public GameObject BlueSample;
    public GameObject RedSample;

    public Slider W0;
    public Slider W1;
    public Slider B0;

    public Button LearnBtn;

    public GameObject DecisionBoundaryGO;
    private LineRenderer DecisionBoundary;

    private float MaxX = 100f;
    private float MinX = -100f;

    private float MaxY = 100f;
    private float MinY = -100f;

    private void Start()
    {
        DecisionBoundary = DecisionBoundaryGO.GetComponent<LineRenderer>();
    }

    private (Vector3, Vector3) CalculateDecisionBoundary()
    {

        var x = W0.value;
        var y = W1.value;
        var b = B0.value;

        if(y == 0 && x == 0)
        {
            return new(
                new Vector3(0, 0 + b, -2),
                new Vector3(0, 0 + b, -2)
            );
        }

        if (y == 0)
        {
            return new(
                new Vector3(0, MinY + b, -2),
                new Vector3(0, MaxY + b, -2)
            );
        }

        if (x == 0)
        {
            return new(
                new Vector3(MinX, 0 + b, -2),
                new Vector3(MaxX, 0 + b, -2)
            );
        }

        return new(
            new Vector3(MinX, -MinX*x / y + B0.value, -2f),
            new Vector3(MaxX, -MaxX*x / y + B0.value, -2f)
       );
    }


    // Update is called once per frame
    void Update()
    {

        if(RedParent.transform.childCount != 0 && BlueParent.transform.childCount != 0)
        {
            LearnBtn.interactable = true;
        }
        else
        {
            LearnBtn.interactable = false;
        }

        var start = CalculateDecisionBoundary().Item1;
        var end = CalculateDecisionBoundary().Item2;

        DecisionBoundary.SetPositions(new Vector3[] { 
            start,
            end
        });

        // Left Click: spawn red sample
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.point.x > MaxX || hit.point.x < MinX || hit.point.y > MaxY || hit.point.y < MinY)
                {
                    Debug.Log("halo!");
                }
                else
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -2);
                    Instantiate(RedSample, hit.point, Quaternion.identity, RedParent.transform);
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
                if (hit.point.x > MaxX || hit.point.x < MinX || hit.point.y > MaxY || hit.point.y < MinY)
                {
                    Debug.Log("halo!");
                }
                else
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -2);
                    Instantiate(BlueSample, hit.point, Quaternion.identity, BlueParent.transform);
                }
            }
        }
    }

    public void ClearSamples()
    {
        for(int i = 0; i < RedParent.transform.childCount; i++)
        {
            Destroy(RedParent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < BlueParent.transform.childCount; i++)
        {
            Destroy(BlueParent.transform.GetChild(i).gameObject);
        }
    }

    private float gradientW0;
    private float gradientW1;
    private float gradientB0;

    public void Learn()
    {
        const float h = 0.0001f;
        const float lr = 0.001f;
        var originalLoss = Loss();

        for (int i = 0; i < 1000; i++)
        {
            W0.value += h;
            var deltaCost = Loss() - originalLoss;
            W0.value -= h;
            gradientW0 = deltaCost / h;


            W1.value += h;
            deltaCost = Loss() - originalLoss;
            W1.value -= h;
            gradientW1 = deltaCost / h;


            B0.value += h;
            deltaCost = Loss() - originalLoss;
            B0.value -= h;
            gradientB0 = deltaCost / h;

            W0.value -= gradientW0 * lr;
            W1.value -= gradientW1 * lr;
            B0.value -= gradientB0 * lr;

            originalLoss = Loss();
        }
    }

    private float Sigmoid(float x)
    {
        return 1 / ( 1 + Mathf.Exp(-x) );
    }

    private float log10(float x)
    {
        if (x != 0)
        {
            return Mathf.Log10(x);
        }

        return float.MinValue;
    }

    public float Loss()
    {
        var loss = 0f;

        for (int i = 0; i < RedParent.transform.childCount; i++)
        {
            var y = 1f; // true label

            var output = W0.value * RedParent.transform.GetChild(i).transform.position.x 
                + W1.value * RedParent.transform.GetChild(i).transform.position.y
                + B0.value;

            output = Sigmoid(output);
            //Debug.Log("red: " + output + ", should be one.");
            loss += -y * log10(output) - ( 1 - y ) * log10(1 - output);
        }

        //Debug.Log("loss after red: " + loss);

        for (int i = 0; i < BlueParent.transform.childCount; i++)
        {
            var y = 0f; // true label

            var output = W0.value * BlueParent.transform.GetChild(i).transform.position.x
                + W1.value * BlueParent.transform.GetChild(i).transform.position.y
                + B0.value;

            output = Sigmoid(output);
            //Debug.Log("blue: " + output + ", should be zero.");
            loss += -y * log10(output) - (1 - y) * log10(1 - output);
        }

        //Debug.Log(loss/(RedParent.transform.childCount + BlueParent.transform.childCount)); // Loss func is 1/N * (-y * log(y^) - (1-y) * log(1-y^))
        return loss;
    }
}
