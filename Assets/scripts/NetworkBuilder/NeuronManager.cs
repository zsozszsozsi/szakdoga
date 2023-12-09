using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NeuronManager : MonoBehaviour
{
    private Controller Controller;

    // Start is called before the first frame update
    void Start()
    {
        Controller = Controller.Instance;
    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Controller.AddNeuron(transform);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            Controller.RemoveNeuron(transform);
        }
    }
}
