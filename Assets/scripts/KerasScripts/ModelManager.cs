using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow.NumPy;
using System.Linq;
using Unity.VisualScripting;
using System.Drawing.Printing;
using System;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System.Xml.Linq;

public class ModelManager : MonoBehaviour
{
    public ModelBase Model;

    public static ModelManager Instance;

    public Slider EpochSlider;
    public TextMeshProUGUI EpochData;
    public Button StartTrainBtn;
    public Button ResetBtn;

    [TextArea(3, 10)]
    public string mlp_desc;
    [TextArea(3, 10)]
    public string lenet5_desc;

    public TextMeshProUGUI NetworkDesc;

    public GameObject TrainLog;
    public TextMeshProUGUI train_log;

    [HideInInspector]
    public bool IsSetupDone = false;

    private int iterations = 10;
    private bool startTrain = false;

    private NDArray x_train, y_train, x_test, y_test;
    public NDArray X_test, Y_test;

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

    public void Start()
    {
        StartTrainBtn.onClick.AddListener(() => StartTrain());
        ResetBtn.interactable = false;
        ResetBtn.onClick.AddListener(() => ResetModel());

        (x_train, y_train, x_test, y_test) = keras.datasets.mnist.load_data();

        X_test = x_test.reshape((10000, 784))[":3000"] / 255f;
        Y_test = y_test[":3000"];

        Model = new Fnn(x_train, y_train, x_test, y_test);

        NetworkDesc.text = mlp_desc;

        IsSetupDone = true;
    }

    public void StartTrain()
    {
        startTrain = true;
        StartTrainBtn.interactable = false;
        EpochSlider.interactable = false;

        for(int i = 0; i < TrainLog.transform.childCount; i++)
        {
            Destroy(TrainLog.transform.GetChild(i).gameObject);
        }
    }

    public void ResetModel()
    {
        Model.BuildModel();
        startTrain = false;

        counter = 0;

        StartTrainBtn.interactable = true;
        EpochSlider.interactable = true;
        ResetBtn.interactable = false;
    }


    public void ChangeModel(int val)
    {
        if( val == 0)
        {
            Model = new Fnn(x_train, y_train, x_test, y_test);
            NetworkDesc.text = mlp_desc;
        }
        else if(val == 1)
        {
            Model = new LeNet5(x_train, y_train, x_test, y_test);
            NetworkDesc.text = lenet5_desc;
        }
    }


    int counter = 0;
    private void Update()
    {
        if (!startTrain)
        {
            iterations = (int)EpochSlider.value;
            EpochData.text = iterations.ToString();
        }

        if(startTrain && counter < iterations)
        {
            var res = Model.Train();
            counter++;
            train_log.text = $"[Epoch {counter}] : {res}";
            Instantiate(train_log, TrainLog.transform);
        }
        else if(startTrain)
        {
            ResetBtn.interactable = true;
        }
    }
}

public abstract class ModelBase 
{
    public ModelBase(NDArray x_train, NDArray y_train, NDArray x_test, NDArray y_test)
    {
        this.x_train = x_train;
        this.y_train = y_train;

        this.x_test = x_test;
        this.y_test = y_test;
    }

    public NDArray x_train, y_train, x_test, y_test;
    public Model Model;

    public abstract void PrepareData();
    public abstract void BuildModel();
    public abstract string Train();
    public abstract void Test(int predIdx);
    public abstract Dictionary<int, float> Predict(NDArray pred);
}

public class LeNet5 : ModelBase
{
    public LeNet5(NDArray x_train, NDArray y_train, NDArray x_test, NDArray y_test): base(x_train, y_train, x_test, y_test)
    {
        PrepareData();
        BuildModel();
    }

    public override void PrepareData()
    {
        x_train = x_train.reshape((x_train.shape[0], x_train.shape[1], x_train.shape[2], 1))[$":30000"] / 255f; // shape: {60000, 28,28} -> {60000, 28,28, 1}
        x_test = x_test.reshape((x_test.shape[0], x_test.shape[1], x_test.shape[2], 1))[":100"] / 255f;

        y_train = y_train[":30000"];
        y_test = y_test[":100"];
    }

    public override void BuildModel()
    {
        var inputs = keras.Input(shape: (28, 28, 1));

        var layers = new LayersApi();

        var outputs = layers.Conv2D(filters: 6, kernel_size: (5, 5), activation: keras.activations.Relu)
            .Apply(inputs);
        outputs = layers.MaxPooling2D(pool_size: (2, 2)).Apply(outputs);

        outputs = layers.Conv2D(filters: 16, kernel_size: (5, 5), activation: keras.activations.Relu)
            .Apply(outputs);
        outputs = layers.MaxPooling2D(pool_size: (2, 2)).Apply(outputs);

        outputs = layers.Flatten().Apply(outputs);
        outputs = layers.Dense(120, activation: keras.activations.Relu).Apply(outputs);
        outputs = layers.Dense(84, activation: keras.activations.Relu).Apply(outputs);
        outputs = layers.Dense(10, activation: keras.activations.Softmax).Apply(outputs);

        Model = keras.Model(inputs, outputs, name: "mnist_model_LeNet5");
        Model.summary();

        Model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            optimizer: keras.optimizers.Adam(),
            metrics: new[] { "accuracy" });
    }

    public override string Train()
    {
        var history = Model.fit(x_train, y_train, batch_size: 64, epochs: 1).history;
        string res = "";

        foreach (var item in history)
        {
            string s = item.Key + ": ";
            foreach (var loss in item.Value)
            {
                s += loss + " ,";
            }
            s = s[..^1];
            res += s;
        }

        return string.Join(" ", res.Split('\n'));
    }

    public override void Test(int predIdx)
    {
        throw new NotImplementedException();
    }

    public override Dictionary<int, float> Predict(NDArray pred)
    {
        var predTensors = Model.predict(pred.reshape((1, 28, 28, 1)));
        var predictions = predTensors[0].numpy().reshape(-1);
        
        var result = new Dictionary<int, float>();

        for(int i = 0; i < (int)predictions.size; i++)
        {
            result.Add(i, predictions[i]);
        }

        return result.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

}

public class Fnn : ModelBase
{
    public NDArray y_train_onehot, y_test_onehot;

    public Fnn(NDArray x_train, NDArray y_train, NDArray x_test, NDArray y_test) : base(x_train, y_train, x_test, y_test)
    {
        PrepareData();
        BuildModel();
    }

    public override void PrepareData()
    {
        x_train = x_train.reshape((60000, 784))[$":30000"] / 255f;
        x_test = x_test.reshape((10000, 784))[":3000"] / 255f;

        y_train = y_train[":30000"];
        y_test = y_test[":3000"];

        y_train_onehot = np.zeros(((int)y_train.size, 10), dtype: Tensorflow.TF_DataType.TF_FLOAT);
        y_train_onehot[np.arange((int)y_train.size), y_train] = 1f;

        y_test_onehot = np.zeros(((int)y_test.size, 10), dtype: Tensorflow.TF_DataType.TF_FLOAT);
        y_test_onehot[np.arange((int)y_test.size),y_test] = 1f;

    }

    public override void BuildModel()
    {
        var inputs = keras.Input(shape: 784);

        var layers = new LayersApi();

        var outputs = layers.Dense(32, activation: keras.activations.Relu).Apply(inputs);
        outputs = layers.Dense(10, activation: keras.activations.Softmax).Apply(outputs);

        Model = keras.Model(inputs, outputs, name: "mnist_model");
        Model.summary();

        Model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            optimizer: keras.optimizers.Adam(),
            metrics: new[] { "accuracy" });
    }

    public override string Train()
    {
        var history = Model.fit(x_train, y_train, batch_size: 64, epochs: 1).history;
        //model.evaluate(x_test, y_test, return_dict: true);
        string res = "";
        foreach(var item in history)
        {
            string s = item.Key + ": ";
            foreach(var loss in item.Value)
            {
                s += loss + " ,";
            }
            s = s[..^1];
            //Debug.Log(s);
            res += s + "\n";
        }

        return string.Join(" ", res.Split('\n'));
    }

    public override void Test(int predIdx)
    {
        var predTensors = Model.predict(x_test[predIdx].reshape((1, -1)));
        var pred = predTensors[0].numpy().reshape(-1);

        float max = pred[0];
        int maxId = 0;

        for (int i = 1; i < 10; i++)
        {
            if (pred[i] > max)
            {
                max = pred[i];
                maxId = i;
            }
        }

        Debug.Log($"Predicted value: {maxId}, true val: {y_test[predIdx]}");
        
    }

    public override Dictionary<int, float> Predict(NDArray pred)
    {
        var predTensors = Model.predict(pred.reshape((1, -1)));
        var predictions = predTensors[0].numpy().reshape(-1);

        var result = new Dictionary<int, float>();

        for (int i = 0; i < (int)predictions.size; i++)
        {
            result.Add(i, predictions[i]);
        }

        return result.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x=> x.Value);
    }
}
