using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
public class DecisionBoundary : MonoBehaviour
{
    public float MinX = 0f;
    public float MaxX = 0f;
    [Range(0.05f, 0.5f)]
    public float Step = 0.01f;

    private LineRenderer LineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
      
    }

    public void DrawDecisionBoundary(float w1, float w2, float b)
    {
        LineRenderer.positionCount = 2;

        float y = -(b + w1 * MinX) / w2;
        LineRenderer.SetPosition(0, new Vector3(MinX, y, -1f));

        y = -(b + w1 * MaxX) / w2;
        LineRenderer.SetPosition(1, new Vector3(MaxX, y, -1f));
    }

    public void DrawDecisionBoundary(NeuralNetwork netrowk)
    {
        //max x = 3.45
        //min x = -3.35
        //max y = 1.45
        //min y = -1.40

        List<Vector3> points = new List<Vector3>();
        string tmp = "";

        for (float i = -3.35f; i <= 3.45f; i+=Step)
        {
           for (float j = -1.4f; j <= 1.45f; j+=Step)
           {
                var output = netrowk.FeedForward(i, j);
                tmp += $"({i};{j})={output}\t";

                if (Mathf.Abs(output - 0.5f) < 0.003f)
                {
                    points.Add(new Vector3(i, j, -1.5f));
                }
           }
            tmp += "\n";
        }
        //Debug.Log(tmp);
        LineRenderer.positionCount = points.Count;
        LineRenderer.SetPositions(points.ToArray());
    }
}
