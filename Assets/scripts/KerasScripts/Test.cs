using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Layers;
using static Tensorflow.KerasApi;
using Tensorflow.NumPy;
using System.Linq;

public class Test : MonoBehaviour
{
    Fnn fnn = new();
    LeNet5 cnn = new();

    public void Start()
    {
        fnn.PrepareData();
        fnn.BuildModel();
    }

    float time = 0f;
    float counter = 0f;
    private void Update()
    {
        time += Time.deltaTime;
        
        if(time > 0.25f && counter < 10)
        {
            time = 0f;

            //cnn.Train();
            fnn.Train();
            counter++;
        }

        if(counter == 30 && time > 0.5f)
        {
            time = 0f;
            var idx = Mathf.RoundToInt(Random.Range(0, fnn.y_test.size));
            fnn.Test(idx);
        }
    }
}

public class LeNet5
{
    public Model model;
    public Tensorflow.NumPy.NDArray x_train, y_train, x_test, y_test;

    public LeNet5()
    {
        PrepareData();
        BuildModel();
    }

    public void PrepareData()
    {
        (x_train, y_train, x_test, y_test) = keras.datasets.mnist.load_data();
        x_train = x_train.reshape((x_train.shape[0], x_train.shape[1], x_train.shape[2], 1))[$":1000"] / 255f; // shape: {60000, 28,28} -> {60000, 28,28, 1}
        x_test = x_test.reshape((x_test.shape[0], x_test.shape[1], x_test.shape[2], 1))[":100"] / 255f;

        y_train = y_train[":1000"];
        y_test = y_test[":100"];
    }

    public void BuildModel()
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
        outputs = layers.Dense(10, activation: keras.activations.Softmax).Apply(outputs);

        model = keras.Model(inputs, outputs, name: "mnist_model_LeNet5");
        model.summary();

        model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            optimizer: keras.optimizers.Adam(),
            metrics: new[] { "accuracy" });
    }

    public void Train()
    {
        var history = model.fit(x_train, y_train, batch_size: 64, epochs: 1).history;

        foreach (var item in history)
        {
            string s = item.Key + ": ";
            foreach (var loss in item.Value)
            {
                s += loss + " ,";
            }
            s = s[..^1];
            Debug.Log(s);
        }
    }

}

public class Fnn
{
    public Model model;
    public Tensorflow.NumPy.NDArray x_train, y_train, x_test, y_test;
    public NDArray y_train_onehot, y_test_onehot;

    public void PrepareData()
    {
        (x_train, y_train, x_test, y_test) = keras.datasets.mnist.load_data();
        x_train = x_train.reshape((60000, 784))[$":1000"] / 255f;
        x_test = x_test.reshape((10000, 784))[":100"] / 255f;

        y_train = y_train[":1000"];
        y_test = y_test[":100"];

        y_train_onehot = np.zeros(((int)y_train.size, 10), dtype: Tensorflow.TF_DataType.TF_FLOAT);
        y_train_onehot[np.arange((int)y_train.size), y_train] = 1f;
        Debug.Log(y_train_onehot[0]);

        y_test_onehot = np.zeros(((int)y_test.size, 10), dtype: Tensorflow.TF_DataType.TF_FLOAT);
        y_test_onehot[np.arange((int)y_test.size),y_test] = 1f;

    }

    public void BuildModel()
    {
        var inputs = keras.Input(shape: 784);

        var layers = new LayersApi();

        var outputs = layers.Dense(64, activation: keras.activations.Relu).Apply(inputs);
        outputs = layers.Dense(10, activation: keras.activations.Softmax).Apply(outputs);

        model = keras.Model(inputs, outputs, name: "mnist_model");
        model.summary();

        model.compile(loss: keras.losses.SparseCategoricalCrossentropy(from_logits: true),
            optimizer: keras.optimizers.Adam(),
            metrics: new[] { "accuracy" });
    }

    public void Train()
    {
        var history = model.fit(x_train, y_train, batch_size: 64, epochs: 1).history;
        //model.evaluate(x_test, y_test, return_dict: true);

        foreach(var item in history)
        {
            string s = item.Key + ": ";
            foreach(var loss in item.Value)
            {
                s += loss + " ,";
            }
            s = s[..^1];
            Debug.Log(s);
        }
    }

    public void Test(int predIdx)
    {
        var predTensors = model.predict(x_test[predIdx].reshape((1, -1)));
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
}
