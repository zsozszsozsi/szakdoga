using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
public class DecisionBoundary : MonoBehaviour
{
    private Simulator Simulator;

    private float Step = 0.03f;

    public GameObject Plane;
    private Texture2D Texture;

    // Start is called before the first frame update
    void Start()
    {
        Simulator = Simulator.Instance;

        int height = Mathf.CeilToInt(Simulator.MaxY * 2 / Step);
        int width = Mathf.CeilToInt(Simulator.MaxX * 2 / Step);
        

        Texture = new Texture2D(width, height);

        Plane.GetComponent<Renderer>().material.mainTexture = Texture;
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
