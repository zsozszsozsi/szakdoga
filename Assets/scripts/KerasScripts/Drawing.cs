using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow.NumPy;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Security.Cryptography;

public class Drawing : MonoBehaviour
{
    public static Drawing Instance;

    public RawImage Pixel;
    public GridLayoutGroup Grid;
    public TextMeshProUGUI PredictText;

    public GameObject PredictionLog;
    public TextMeshProUGUI prediction_log;

    [HideInInspector]
    public List<RawImage> pixels = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var x = Grid.cellSize.x;
        var width = gameObject.GetComponent<RectTransform>().rect.width;

        int count = (int)Mathf.Round(width / x) * (int)Mathf.Round(width / x);
        print((int)Mathf.Round(width / x) + "x" + (int)Mathf.Round(width / x));

        int row = 0;
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(Pixel, transform);
            if ((i + 1) % 28 == 0) row++;
            go.name = $"{row};{i%28}";
            pixels.Add(go.GetComponent<RawImage>());
        }

    
    }

    public List<(int, int)> GetNeihgbours(int row, int col)
    {
        var coords = new List<(int, int)>() { (row, col-1), /*(row-1, col-1),*/ (row-1, col), /*(row-1, col+1),*/ (row, col+1), /*(row+1, col+1),*/ (row+1, col), /*(row+1, col-1)*/};

        return coords.Where(c => c.Item1 >= 0 && c.Item1 <= 27 && c.Item2 >= 0 && c.Item2 <= 27).ToList();
    }

    public static int CoordToIndex((int, int) coord)
    {
        return coord.Item1 * 28 + coord.Item2;
    }

    public void Predict()
    {
        NDArray pred = np.zeros(784, dtype: Tensorflow.TF_DataType.TF_FLOAT);


        for (int i = 0; i < pixels.Count; i++)
        {
            pred[i] = pixels[i].color.grayscale;
        }

        //int index = Mathf.RoundToInt(Random.Range(0, 3000));

        //var predicted = ModelManager.Instance.Model.Predict(ModelManager.Instance.X_test[index]);
        var predicted = ModelManager.Instance.Model.Predict(pred);
        //InspectorManager.Instance.Draw(ModelManager.Instance.X_test[index], predicted.ToString());
        //InspectorManager.Instance.Draw(pred, predicted.First().Key.ToString());

        PredictText.text = $"Prediction: {predicted.First().Key}";

        for (int i = 0; i < PredictionLog.transform.childCount; i++)
        {
            Destroy(PredictionLog.transform.GetChild(i).gameObject);
        }

        foreach(var predDict in predicted)
        {
            prediction_log.text = $"{predDict.Key} => {predDict.Value * 100:0.00}%";
            Instantiate(prediction_log, PredictionLog.transform);
        }

    }


}
