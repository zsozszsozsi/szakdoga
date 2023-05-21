using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
public class DecisionBoundary : MonoBehaviour
{
    private Simulator Simulator;

    [Range(0.1f, 0.5f)]
    public float Step = 0.1f;

    private LineRenderer LineRenderer;
    public GameObject Plane;
    private Texture2D Texture;

    // Start is called before the first frame update
    void Start()
    {
        Simulator = Simulator.Instance;

        LineRenderer = GetComponent<LineRenderer>();
        int width = 10;
        int height = Mathf.CeilToInt(width * (Plane.transform.localScale.y / Plane.transform.localScale.x));
        Texture = new Texture2D(width, height);

        Plane.GetComponent<Renderer>().material.mainTexture = Texture;
    }

    public void DrawDecisionBoundary(float w1, float w2, float b)
    {
        LineRenderer.positionCount = 2;

        float y = -(b + w1 * Simulator.MinX) / w2;
        LineRenderer.SetPosition(0, new Vector3(Simulator.MinX, y, -1f));

        y = -(b + w1 * Simulator.MaxX) / w2;
        LineRenderer.SetPosition(1, new Vector3(Simulator.MaxX, y, -1f));
    }

    public void DrawDecisionBoundaryWithText(NeuralNetwork network)
    {
        for (float i = Simulator.MinX; i <= Simulator.MaxX; i += Step)
        {
            for (float j = Simulator.MinY; j <= Simulator.MaxY; j += Step)
            {
                var x = Mathf.CeilToInt((i - Simulator.MinX) / (Simulator.MaxX - Simulator.MinX) * Texture.width);
                var y = Mathf.CeilToInt((j - Simulator.MinY) / (Simulator.MaxY - Simulator.MinY) * Texture.height);
                
                var output = network.FeedForward(i, j);
                var color = new Color(Mathf.Lerp(0, 1, output), 0, Mathf.Lerp(1, 0, output));
                Texture.SetPixel(Texture.width - x, Texture.height - y, color);
            }
        }

        Texture.Apply();

    }
}
