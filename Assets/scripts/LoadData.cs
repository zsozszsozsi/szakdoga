using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using System.Globalization;


public class LoadData : MonoBehaviour
{
    public GameObject tmp;
    public GameObject canvas;
    public RawImage img;
    public Text label_text;

    public GameObject Cube;
    public GameObject Spawner;
    

    private List<Texture2D> textureList = new ();
    private float[,] data;
    private List<int> labels = new ();

    private List<Color> ColorMap = new () {
        new Color32(129, 247, 166, 255), // 0
        new Color32(247, 150, 129, 255), // 1
        new Color32(129, 210, 247, 255), // 2
        new Color32(202, 129, 247, 255), // 3
        new Color32(247, 129, 184, 255), // 4
        new Color32(252, 223,  78, 255), // 5
        new Color32(252, 168,  78, 255), // 6
        new Color32(215, 252,  78, 255), // 7
        new Color32( 78, 180, 252, 255), // 8
        new Color32(252,  58,  65, 255)  // 9
    };

    public void TestBtn()
    {

        var path = EditorUtility.OpenFilePanel("Open csv", "", "csv");
        var stringData = System.IO.File.ReadAllText(path);
        List<string> lines = stringData.Split("\n").ToList();
       
        var emptyCount = lines.Where(row => string.IsNullOrEmpty(row) == true).Count() +1;

        data = new float[lines.Count-emptyCount, lines[0].Split(",").Length-2];

        for(int i = 1; i < lines.Count; i++)
        {
            var words = lines[i].Split(",");

            if(int.TryParse(words[0].Trim(), out var labelId))
            {
                labels.Add(labelId);
            }

            for(int j = 2; j < words.Length; j++)
            {
                if (float.TryParse(words[j].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var pixel))
                {
                    data[i - 1, j - 2] = pixel;
                }
            }

        }


        // PCA
        var pca = new PCA(data, 3);
        pca.Compute();

        Debug.Log("MNIST LOADED!");

        canvas.GetComponent<Canvas>().enabled = false;       
        
        for(int i = 0; i < pca.TransformedData.GetLength(0); i++)
        {
            var texture = new Texture2D(28, 28, TextureFormat.RGB24, false);
            int rowCount = 0;
            for(int j = 0; j < data.GetLength(1); j++)
            {
                if(j%28 == 0)
                {
                    rowCount++;
                }
                texture.SetPixel(rowCount, j % 28, new Color( data[i,j], data[i, j], data[i, j]));
            }
            texture.Apply();
            textureList.Add(texture);
        }

        for(int i = 0; i < pca.TransformedData.GetLength(0); i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = Spawner.transform;
            cube.transform.localScale = new Vector3(28f, 28f, 0.1f);
            cube.transform.position = new Vector3(pca.TransformedData[i, 0], pca.TransformedData[i, 1], pca.TransformedData[i, 2]);
            cube.GetComponent<Renderer>().material.mainTexture = textureList[i];
            cube.GetComponent<Renderer>().material.color = ColorMap[labels[i]];
            
        }
        
    }

    private void Update()
    {

        for(int i = 0; i < Spawner.transform.childCount; i++)
        {
            var cube = Spawner.transform.GetChild(i);
            cube.LookAt(Camera.main.transform.position);
        }
        
    }
}
