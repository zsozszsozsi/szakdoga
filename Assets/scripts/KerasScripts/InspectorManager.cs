using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow.NumPy;
using TMPro;
using Unity.Collections;
using System.Threading.Tasks;

public class InspectorManager : MonoBehaviour
{
    public static InspectorManager Instance;

    public RawImage Image;
    public TextMeshProUGUI LabelText;
    public TextMeshProUGUI IndexLabel;

    private ModelManager ModelManager;
    private int index = 0;

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
    async void Start()
    {
        ModelManager = ModelManager.Instance;

        await Task.Run(() => {
            while (!ModelManager.IsSetupDone) { }
        });

        LabelText.text = "Label: " + ModelManager.Model.y_test[index];
        IndexLabel.text = $"{index + 1} / {ModelManager.Model.x_test.shape[0]}";
        Draw();
    }

    public void GoLeft()
    {
        if (index != 0)
        {
            index--;
            IndexLabel.text = $"{index+1} / {ModelManager.Model.x_test.shape[0]}";
            Draw();
        }

    }

    public void GoRight()
    {
        if (index < (int)ModelManager.Model.x_test.size)
        {
            index++;
            IndexLabel.text = $"{index + 1} / {ModelManager.Model.x_test.shape[0]}";
            Draw();
        }
    }
    public void Draw()
    {
        var data = ModelManager.Model.x_test[index] * 255;

        var texture = new Texture2D(28, 28, TextureFormat.RGB24, false);

        int rowCount = 27;
        for (int i = 0; i < (int)data.size; i++)
        {
            if ((i + 1) % 28 == 0)
            {
                rowCount--;
            }
            texture.SetPixel(i % 28, rowCount, new Color(data[i], data[i], data[i]));
            
        }

        LabelText.text = "Label: " + ModelManager.Model.y_test[index];

        texture.SetPixel(0, 0, Color.red);
        texture.SetPixel(0, 27, Color.green);
        texture.SetPixel(27, 0, Color.blue);
        texture.SetPixel(27, 27, Color.black);
        

        texture.Apply();

        Image.texture = texture;
    }

    public void Draw(NDArray arr)
    {
        var data = ModelManager.Model.x_test[index] * 255;

        var texture = new Texture2D(28, 28, TextureFormat.RGB24, false);

        int rowCount = 27;
        for (int i = 0; i < (int)arr.size; i++)
        {
            if ((i + 1) % 28 == 0)
            {
                rowCount--;
            }
            texture.SetPixel(i % 28, rowCount, new Color(arr[i], arr[i], arr[i]));

        }

        LabelText.text = "Label: " + ModelManager.Model.y_test[index];

        texture.SetPixel(0, 0, Color.red);
        texture.SetPixel(0, 27, Color.green);
        texture.SetPixel(27, 0, Color.blue);
        texture.SetPixel(27, 27, Color.black);


        texture.Apply();

        Image.texture = texture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
