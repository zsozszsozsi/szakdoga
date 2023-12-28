using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Globalization;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using System;

public class LoadData : MonoBehaviour
{
    public GameObject SetupScene;
    public GameObject ParamsScene;
    public GameObject Lab;
    public GameObject Loading;
    public Text LoadingText;
    public Text ErrorText;

    public GameObject GameScene;
    public GameObject Warning;
    public Text Statistics;

    public GameObject Cube;
    public GameObject Spawner;

    public GameObject ObjectToggle;

    private int DimensionCount = 3;

    private List<Texture2D> textureList = new ();
    private float[,] data;
    private List<string> labels = new ();

    private bool IsPCAComputing = false;
    private bool IsCSVLoading = false;
    private bool IsDatasetTooBig = false;

    private HashSet<string> Categories = new();

    private readonly List<Color> ColorMap = new () {
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
    
    public enum LabelPos
    {
        First=1,
        Last=2
    }

    private async Task LoadCSV(string path, LabelPos labelPos)
    {
        string stringData = null;
        try
        {
            stringData = System.IO.File.ReadAllText(path);
        } catch (Exception _)
        {
            ErrorText.text = "[ERROR]: File not found! Press ESC and go back to main menu!";
        }

        if(stringData == null)
        {
            await Task.Run(() => { while (true) ; });
        }

        List<string> lines = stringData.Split("\n").ToList();

        var emptyCount = lines.Where(row => string.IsNullOrEmpty(row)).Count() + 1;

        data = new float[lines.Count - emptyCount, lines[0].Split(",").Length - 1];
        textureList = new();
        Categories = new();
        labels = new();

        var sw = Stopwatch.StartNew();
        await Task.Run(() =>
        {
            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;

                var words = lines[i].Split(",");

                int startIndex = 0;
                int endIndex = 0;
                int modifier = 0;

                if (labelPos == LabelPos.First)
                {
                    startIndex = 1;
                    endIndex = words.Length;
                    modifier = -1;

                    labels.Add(words[0].Trim());

                    if (!Categories.Contains(labels[^1])) Categories.Add(labels[^1]);
                }
                else
                {
                    startIndex = 0;
                    endIndex = words.Length-1;

                    labels.Add(words[^1].Trim());

                    if (!Categories.Contains(labels[^1])) Categories.Add(labels[^1]);
                }

                for (int j = startIndex; j < endIndex; j++)
                {
                    if (float.TryParse(words[j].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    {
                        data[i - 1, j + modifier] = val;
                    }
                }

            }
        });

        Categories = Categories.OrderBy(x => x).ToHashSet();
        Debug.Log($"excel load time: {sw.Elapsed}");
    }

    public async void StartProcessing(string path, int dimensionCount, LabelPos labelPos)
    {
        DimensionCount = dimensionCount;

        ParamsScene.SetActive(false);
        Loading.SetActive(true);

        IsCSVLoading = true;
        await LoadCSV(path, labelPos);
        IsCSVLoading = false;

        IsDatasetTooBig = data.GetLength(0) > 10_000;

        string datasetName = "";
        bool usePreComputerData = true;
        if (path.Contains("/"))
        {
            datasetName = path.Split("/")[^1].Split(".")[0] + "_" + dimensionCount + "D";
        }
        else
        {
            datasetName = path.Split("\\")[^1].Split(".")[0] + "_" + dimensionCount + "D";
            usePreComputerData = false;
        }
        

        var sw = Stopwatch.StartNew();
        // PCA
        sw.Restart();
        var pca = new PCA(data, DimensionCount);
        IsPCAComputing = true;
        await pca.Compute(datasetName, usePreComputedData: usePreComputerData);
        IsPCAComputing = false;
        Debug.Log($"PCA compute time: {sw.Elapsed}");             

        CameraController.IsEnabled = true;

        SetupScene.SetActive(false);
        GameScene.SetActive(true);
        Lab.SetActive(true);

        if (IsDatasetTooBig) Warning.SetActive(true);

        Statistics.text = $"Samples: {data.GetLength(0)}\n" +
            $"Features: {data.GetLength(1)}\n" + 
            $"Dimension: {DimensionCount}\n" +
            $"Variance: {pca.RemainingVariance*100:0.00}%";
        
        for(int i = 0; i < data.GetLength(0); i++) // Row Count 
        {
            var texture = new Texture2D(28, 28, TextureFormat.RGB24, false);

            if(data.GetLength(1) == 28 * 28)
            {
                int rowCount = 27;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if ((j + 1) % 28 == 0)
                    {
                        rowCount--;
                    }
                    texture.SetPixel(j % 28, rowCount, new Color(data[i, j], data[i, j], data[i, j]));
                }
            }
           
            texture.Apply();
            textureList.Add(texture);
        }


        var objectToggleParent = GameScene.transform.Find("ObjectToggles");

        for (int i = 0; i < Categories.Count; i++)
        {
            var category = Categories.ElementAt(i);
            var cat_name = $"{category}_objects";

            GameObject gameObject = new GameObject(cat_name);
            gameObject.transform.SetParent(Spawner.transform);
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
            gameObject.AddComponent<MeshCombiner>();

            var toggle = Instantiate(ObjectToggle, objectToggleParent);
            toggle.GetComponentInChildren<Text>().text = cat_name;
            toggle.transform.position = new Vector2(toggle.transform.position.x, toggle.transform.position.y + (i * -40));
            toggle.transform.GetComponent<Toggle>().onValueChanged.AddListener((value) => { HideShowObjects(cat_name, value); });
        }

        for(int i = 0; i < data.GetLength(0); i++)
        {
            GameObject cube = Instantiate(Cube);

            Categories.TryGetValue(labels[i], out var val);
            int index = Categories.ToList().IndexOf(val);

            cube.transform.SetParent(Spawner.transform.GetChild(index));
            cube.transform.localScale = new Vector3(.3f, .3f, .001f);
            cube.transform.position = new Vector3(pca.TransformedData[i, 0], pca.TransformedData[i, 1], DimensionCount == 3 ? pca.TransformedData[i, 2] : 0);
            cube.GetComponent<Renderer>().material.mainTexture = textureList[i];
            cube.GetComponent<Renderer>().material.color = ColorMap[index];
        }

        if (IsDatasetTooBig)
        {
            // Combine all the meshes into one per category
            for(int i = 0; i < Spawner.transform.childCount; i++)
            {
                Spawner.transform.GetChild(i).GetComponent<MeshCombiner>().Combine();
            }
        }

    }

    public void HideShowObjects(string cat_name, bool value)
    {
        Spawner.transform.Find(cat_name).gameObject.SetActive(value);
    }

    public void BackBtn()
    {
        var objectToggleParent = GameScene.transform.Find("ObjectToggles");

        for (int i = 0; i < Spawner.transform.childCount; i++)
        {
            for (int j = 0; j < Spawner.transform.GetChild(i).childCount; j++)
            {
                var cube = Spawner.transform.GetChild(i).GetChild(j);
                Destroy(cube.gameObject);
            }

            Destroy(objectToggleParent.GetChild(i+1).gameObject); // cause the 0th child is a text we dont want to delete that
            Destroy(Spawner.transform.GetChild(i).gameObject);
        }

        GameScene.SetActive(false);
        Lab.SetActive(false);
        Loading.SetActive(false);
        ParamsScene.SetActive(true);
        SetupScene.SetActive(true);
    }

    private void Update()
    {
        if (IsCSVLoading)
        {
            LoadingText.text = "CSV file is loading ...";
        }

        if (IsPCAComputing)
        {
            LoadingText.text = "PCA is computing ...";
        }

        if (!IsDatasetTooBig)
        {
            for (int i = 0; i < Spawner.transform.childCount; i++)
            {
                for(int j = 0; j < Spawner.transform.GetChild(i).childCount; j++)
                {
                    var cube = Spawner.transform.GetChild(i).GetChild(j);
                    cube.LookAt(Camera.main.transform.position);
                }
            }
        }



    }
}
