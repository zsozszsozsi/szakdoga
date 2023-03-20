using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogRegression : MonoBehaviour
{
    public GameObject BlueParent;
    public GameObject RedParent;


    public GameObject BlueSample;
    public GameObject RedSample;

    private float MaxWidth = 35f;
    private float MinWidth = -80.8f;

    private float MaxHeight = 45.9f;
    private float MinHeight = -38.8f;

    // Update is called once per frame
    void Update()
    {
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
