using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionBoundary : MonoBehaviour
{
    public float MinX = 0f;
    public float MaxX = 0f;
    public float Step = 0.5f;

    private LineRenderer LineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        
    }

    public void DrawDecisionBoundary(float w1, float w2, float b)
    {
        LineRenderer.positionCount = Mathf.CeilToInt((MaxX - MinX) / Step) + 1;

        float x = MinX;
        int i = 0;

        while(x <= MaxX)
        {
            float y = -(b + w1 * x) / w2;
            LineRenderer.SetPosition(i, new Vector3(x, y, -1f));

            x += Step;
            i++;
        }
    }

    public void DrawDecisionBoundary(float[] weights, float[] biases)
    {

    }
}
