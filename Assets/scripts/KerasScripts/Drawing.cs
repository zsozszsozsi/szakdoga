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

    private List<RawImage> pixels = new();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 784; i++)
        {
            var go = Instantiate(Pixel, transform);
            pixels.Add(go.GetComponent<RawImage>());
        }
    }

    NDArray pred = np.zeros(784, dtype: Tensorflow.TF_DataType.TF_FLOAT);
    public void Predict()
    {
        for(int i = 0; i < pixels.Count; i++)
        {
            var pixel = pixels[i];
            pred[i] = pixel.color == Color.white ? 1f : 0f;
        }

        print(ModelManager.Instance.Model.Predict(pred));
    }

}
