using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LayerAdder : MonoBehaviour
{
    private Controller Controller;

    void Start()
    {
        Controller = Controller.Instance;
    }

    public void Add()
    {
        Controller.AddLayer();
    }

    public void Remove()
    {
        Controller.RemoveLayer();
    }

}
