using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow.NumPy;

public class Drawing : MonoBehaviour
{
    public RawImage Pixel;
    public GridLayoutGroup Grid;

    private List<RawImage> pixels = new();

    // Start is called before the first frame update
    void Start()
    {
        var x = Grid.cellSize.x;
        var width = gameObject.GetComponent<RectTransform>().rect.width;

        int count = (int)Mathf.Round(width / x) * (int)Mathf.Round(width / x);
        print((int)Mathf.Round(width / x) + "x" + (int)Mathf.Round(width / x));

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(Pixel, transform);
            pixels.Add(go.GetComponent<RawImage>());
        }
    }

    public void Predict()
    {
        NDArray pred = np.zeros(784, dtype: Tensorflow.TF_DataType.TF_FLOAT);

        for (int i = 0; i < pixels.Count; i++)
        {
            pred[i] = pixels[i].color == Color.white ? 255f : 0f;
        }
   
        InspectorManager.Instance.Draw(pred);
        print(ModelManager.Instance.Model.Predict(pred));
    }


}
