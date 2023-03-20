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

    public GameObject DecisionBoundaryGO;
    private LineRenderer DecisionBoundary;

    private float MaxWidth = 35f;
    private float MinWidth = -80.8f;

    private float MaxHeight = 45.9f;
    private float MinHeight = -38.8f;


    private void Start()
    {
        DecisionBoundary = DecisionBoundaryGO.GetComponent<LineRenderer>();
    }

    private (float, float) CalculateDecisionBoundary()
    {   
        if(W0.value == 0 || W1.value == 0)
        {
            return new (0, 0);
        }

        if(B0.value == 0)
        {

        }

        float x = -( B0.value / W1.value) / (B0.value / W0.value);
        float y = -B0.value / W1.value;

        return new (x, y);
    }


    // Update is called once per frame
    void Update()
    {
        var x = CalculateDecisionBoundary().Item1;
        var y = CalculateDecisionBoundary().Item2;

        DecisionBoundary.SetPositions(new Vector3[] { 
            new Vector3(x*MaxWidth, y*MaxHeight, -2),
            new Vector3(x*MinWidth, y*MinHeight, -2) 
        });

        // Left Click: spawn red sample
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.point.x > MaxWidth || hit.point.x < MinWidth || hit.point.y > MaxHeight || hit.point.y < MinHeight)
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
                if (hit.point.x > MaxWidth || hit.point.x < MinWidth || hit.point.y > MaxHeight || hit.point.y < MinHeight)
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
}
