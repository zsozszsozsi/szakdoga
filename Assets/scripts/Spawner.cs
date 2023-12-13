using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        // Left Click: spawn red sample
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (ValidateClick(hit.point))
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -1);
                    Simulator.Instance.InstantiateSample(RedSample, hit.point);

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
                if (ValidateClick(hit.point))
                {
                    hit.point = new Vector3(hit.point.x, hit.point.y, -1);
                    Simulator.Instance.InstantiateSample(BlueSample, hit.point);
                }

            }
        }

    }

    private bool ValidateClick(Vector3 pos)
    {
        return pos.x < Simulator.Instance.MaxX && pos.x > Simulator.Instance.MinX
                    && pos.y < Simulator.Instance.MaxY && pos.y > Simulator.Instance.MinY && !EventSystem.current.IsPointerOverGameObject();
    }

    public void Pattern1()
    {
        for(float i = Simulator.MinX; i < 0f; i += Step)
        {
            for(float j = Simulator.MinY; j < 0f; j += Step)
            {
                if(!Simulator.InstantiateSample(RedSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f)))
                {
                    return;
                }
            }
        }

        for (float i = 0; i < Simulator.MaxX; i += Step)
        {
            for (float j = 0; j < Simulator.MaxY; j += Step)
            {
                if(!Simulator.InstantiateSample(BlueSample, new Vector3(i + Random.Range(0, 0.5f), j + Random.Range(0, 0.5f), -1f)))
                {
                    return;
                }
            }
        }
    }

    public void Pattern2()
    {

        for (float i = Simulator.MinX; i < -0.3f; i += Step)
        {
            for (float j = Simulator.MinY; j < -0.3f; j += Step)
            {
                if (!Simulator.InstantiateSample(RedSample, new Vector3(i + Random.Range(0, 0.2f), j + Random.Range(0, 0.2f), -1f)))
                {
                    return;
                }
            }
        }

        for (float i = 0.3f; i < Simulator.MaxX; i += Step)
        {
            for (float j = 0.3f; j < Simulator.MaxY; j += Step)
            {
                if (!Simulator.InstantiateSample(RedSample, new Vector3(i + Random.Range(0, 0.2f), j + Random.Range(0, 0.2f), -1f)))
                {
                    return;
                }
            }
        }

        for (float i = Simulator.MinX; i < -0.3f; i += Step)
        {
            for (float j = 0.3f; j < Simulator.MaxY; j += Step)
            {
                if (!Simulator.InstantiateSample(BlueSample, new Vector3(i + Random.Range(0, 0.2f), j + Random.Range(0, 0.2f), -1f)))
                {
                    return;
                }
            }
        }

        for (float i = 0.3f; i < Simulator.MaxX; i += Step)
        {
            for (float j = Simulator.MinY; j < -0.3f; j += Step)
            {
                if (!Simulator.InstantiateSample(BlueSample, new Vector3(i + Random.Range(0, 0.2f), j + Random.Range(0, 0.2f), -1f)))
                {
                    return;
                }
            }
        }
    }

    public void Pattern3()
    {
        for (int i = 0; i < 100; i++)
        {
            var center = new Vector3(0, 0, -1f);
            var radius = Random.Range(0f, 0.7f);

            float ang = Random.value * 360;
            
            var x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            var y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

            if (!Simulator.InstantiateSample(RedSample, new Vector3(x, y, -1f)))
            {
                return;
            }
            
        }

        for (int i = 0; i < 100; i++)
        {
            var center = new Vector3(0, 0, -1f);
            var radius = Random.Range(0.75f, 1.5f);

            float ang = Random.value * 360;

            var x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            var y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

            if (!Simulator.InstantiateSample(BlueSample, new Vector3(x, y, -1f)))
            {
                return;
            }

        }
    }

    public void Pattern4()
    {
        // load pre computed principal components
        using (var fileStream = System.IO.File.OpenRead($"Patterns/pattern4.gd"))
        using (var reader = new System.IO.BinaryReader(fileStream))
        {
            var rowCount = reader.ReadInt32();

            List<List<float>> samples = new();

            for (int i = 0; i < rowCount; i++)
            {
                samples.Add(new());
                var colCount = reader.ReadInt32();

                for (int j = 0; j < colCount; j++)
                {
                    
                    samples[i].Add(reader.ReadSingle());
                }
                Simulator.InstantiateSample(samples[i][0] == 1 ? RedSample : BlueSample, new Vector3(samples[i][1], samples[i][2], -1f));
            }
        }
    }
}
